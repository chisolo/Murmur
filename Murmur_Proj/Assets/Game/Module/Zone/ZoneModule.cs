using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lemegeton;

public class ZoneModule : Singleton<ZoneModule>
{
    ZoneModule() {}
    public WorkTeam WorkTeam => _workTeam;
    public List<string> ZonePropTypes => _zonePropTypes;
    private List<string> _zonePropTypes = new List<string>();
    private Dictionary<string, ZoneConfig> _zoneConfig;
    private ZoneArchive _zoneArchive;
    private Dictionary<string, ZoneData> _zones;
    private Dictionary<string, List<ZoneData>> _serivceZones;
    private WorkTeam _workTeam;

    public void Init()
    {
        _zoneConfig = ConfigModule.Instance.Zones();
        _zoneArchive = ArchiveModule.Instance.GetArchive<ZoneArchive>();
        _zones = new Dictionary<string, ZoneData>();
        _serivceZones = new Dictionary<string, List<ZoneData>>();
        var zonePropTypes = typeof(ZoneProperty).GetFields(BindingFlags.Static | BindingFlags.Public);
        foreach(var propType in zonePropTypes) {
            _zonePropTypes.Add(propType.GetValue(null).ToString());
        }
        InitZoneData();
    }

    private void InitZoneData()
    {
        var now = NtpModule.Instance.UtcNowSeconds;
        bool dirty = false;
        foreach(var zoneConfig in _zoneConfig.Values) {
            var zoneId = zoneConfig.id;
            var zoneService = zoneConfig.service;
            ZoneArchiveData zoneArchiveData = _zoneArchive.GetData(zoneId);
            if(zoneArchiveData == null) {
                var zoneItems = new Dictionary<string, int>();
                var initZoneLevel = zoneConfig.init_level > 0 ? 0 : 1;
                foreach(var zoneItem in zoneConfig.items) {
                    zoneItems.Add(zoneItem.id, zoneItem.is_init ? initZoneLevel : 0);
                }
                zoneArchiveData = new ZoneArchiveData(zoneId, zoneConfig.init_level, zoneItems);
                _zoneArchive.AddData(zoneId, zoneArchiveData);
                dirty = true;
            }
            ZoneData zoneData = new ZoneData();
            zoneData.Init(zoneConfig, zoneArchiveData);
            _zones.Add(zoneId, zoneData);
            if(!_serivceZones.ContainsKey(zoneService)) _serivceZones.Add(zoneService, new List<ZoneData>());
            _serivceZones[zoneService].Add(zoneData);
        }
        if(_zoneArchive.work_team.Capacity <= 0) {
            dirty = true;
            _zoneArchive.work_team.AddTeam(GameUtil.DefaultBuildTeam);
        }
        _workTeam = _zoneArchive.work_team;
        _workTeam.SetComplete(FinishLevelupZone);

        foreach(var team in _workTeam.teams) {
            if(!string.IsNullOrEmpty(team.owner) && team.endTime > 0) {
                if(team.endTime <= now) {
                    _zones[team.owner].FinishLevelup();
                    dirty = true;
                } else {
                    _zones[team.owner].Start(team);
                }
            }
        }

        if(dirty) Save();
    }

    public void Save()
    {
        ArchiveModule.Instance.SaveArchive(_zoneArchive);
    }
    public void LevelupZoneItem(string zoneId, string itemId)
    {
        if(_zones.TryGetValue(zoneId, out var zoneData) && zoneData.LevelupZoneItem(itemId)) {
            using (ZoneLevelupEventArgs args = ZoneLevelupEventArgs.Get()) {
                args.zoneId = zoneId;
                args.itemId = itemId;
                EventModule.Instance.FireEvent(EventDefine.ZoneLevelupEvent, args);
            }
            if(zoneData.GetItemData(itemId).Config.property == ZoneProperty.MoneyLimit) {
                PlayerModule.Instance.UpdateMoneyLimit((int)GetTotalProp(ZoneProperty.MoneyLimit));
            }
            Save();
        }
    }
    public void LevelupZone(string zoneId)
    {
        if(_zones.TryGetValue(zoneId, out var zoneData)) {
            if(PlayerModule.Instance.UseMoney((int)zoneData.GetProp(ZoneProperty.LvlUpCost))) {
                var team = _workTeam.GetFreeTeam();
                if(team == null) return;
                team.Begin(zoneId, (int)zoneData.GetProp(BuildingProperty.LvlUpDuration));
                zoneData.Start(team);
                using (ZoneLevelupEventArgs args = ZoneLevelupEventArgs.Get()) {
                    args.zoneId = zoneId;
                    EventModule.Instance.FireEvent(EventDefine.ZoneLevelupEvent, args);
                }
                using (var arg  = EmptyEventArgs.Get()) {
                    EventModule.Instance.FireEvent(EventDefine.BuildingTeamUpdate, arg);
                }
                Save();
            }
        }
    }
    public void FinishLevelupZone(string zoneId)
    {
        if(_zones.TryGetValue(zoneId, out var zoneData) && zoneData.FinishLevelup()) {
            //CalculateGenerateRate();
            _workTeam.WorkDone(zoneId);
            using (ZoneLevelupEventArgs args = ZoneLevelupEventArgs.Get()) {
                args.zoneId = zoneId;
                EventModule.Instance.FireEvent(EventDefine.ZoneLevelupEvent, args);
            }
            using (var arg  = EmptyEventArgs.Get()) {
                EventModule.Instance.FireEvent(EventDefine.BuildingTeamUpdate, arg);
            }
            Save();
        }
    }
    public void ReduceTimeByAd(long time, string zoneId)
    {
        WorkTeamData team = _workTeam.GetTeamData(zoneId);
        if(team != null) {
            team.endTime -= time;
            team.ReduceAd();
            using (ReduceBuildingTimeEventArgs args = ReduceBuildingTimeEventArgs.Get()) {
                args.zoneId = zoneId;
                EventModule.Instance.FireEvent(EventDefine.ReduceBuildingTime, args);
            }
            Save();
        }
    }
    public float GetTotalProp(string prop)
    {
        float total = 0f;
        foreach(var zone in _zones.Values) {
            total += zone.GetProp(prop);
        }
        return total;
    }
    public float GetTotalProp(string service, string prop)
    {
        float total = 0f;
        foreach(var zone in _serivceZones[service]) {
            total += zone.GetProp(prop);
        }
        return total;
    }
    public bool IsZoneLevelFit(ZoneRequire zoneRequire)
    {
        if(string.IsNullOrEmpty(zoneRequire.id)) return true;
        if(_zones.TryGetValue(zoneRequire.id, out var zoneData)) {
            return zoneData.Level >= zoneRequire.level;
        }
        return false;
    }
    #region  team
    public string FindNotOwnTeam()
    {
        return GameUtil.BuildTeamIdList.FirstOrDefault(id => !_workTeam.teams.Any(team => team.id == id));
    }

    public void AddTeam()
    {
        if (_workTeam.IsTeamMax()) {
            return;
        }

        var teamId = FindNotOwnTeam();
        _workTeam.AddTeam(teamId);

        using (var arg  = EmptyEventArgs.Get()) {
            EventModule.Instance.FireEvent(EventDefine.BuildingTeamUpdate, arg);
        }
        Save();
    }

    public void AddTeam(string teamId)
    {
        _workTeam.AddTeam(teamId);
        using (var arg  = EmptyEventArgs.Get()) {
            EventModule.Instance.FireEvent(EventDefine.BuildingTeamUpdate, arg);
        }
        Save();
    }
    #endregion
}