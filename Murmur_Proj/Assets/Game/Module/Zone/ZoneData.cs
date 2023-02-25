using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lemegeton;

public class ZoneData
{
    public ZoneStatus Status => _status;
    public int Level => _archiveData.level;
    public ZoneConfig Config => _config;
    private ZoneConfig _config;
    private ZoneArchiveData _archiveData;
    private ZoneStatus _status;
    private WorkTeamData _team;
    private ToolRequire _toolRequire;
    private Dictionary<string, ZoneItemData> _zoneItemDatas;
    private Dictionary<string, float> _baseProps;
    private Dictionary<string, float> _zoneProps;
    private Dictionary<string, float> _staffProps;
    public void Init(ZoneConfig zoneConfig, ZoneArchiveData zoneArchiveData)
    {
        _config = zoneConfig;
        _archiveData = zoneArchiveData;
        _team = null;
        _toolRequire = new ToolRequire();

        var propTypes = ZoneModule.Instance.ZonePropTypes;
        foreach(var propType in propTypes) {
            _baseProps.Add(propType, 0);
            _zoneProps.Add(propType, 0);
            _staffProps.Add(propType, 0);
        }
        RefreshBaseProps();
        InitZoneItems();
    }

    private void RefreshBaseProps()
    {
        var level = _archiveData.level;
        if(level >= _config.max_level) level = _config.max_level - 1;
        _baseProps[ZoneProperty.LvlUpDuration] = _config.lvlup_duration[level];
        _baseProps[ZoneProperty.LvlUpCost] = _config.lvlup_cost[level];
        _baseProps[ZoneProperty.ServiceEffy] = 0;
        _baseProps[ZoneProperty.MoneyLimit] = 0;
        _baseProps[ZoneProperty.WaitingCapacity] = 0;
        _baseProps[ZoneProperty.ServiceCapacity] = 0;
        _baseProps[ZoneProperty.ProductCapacity] = 0;
        _baseProps[ZoneProperty.Revenue] = _config.revenue;
        _baseProps[ZoneProperty.ItemLvlupDiscount] = 0;
        _baseProps[ZoneProperty.MoveSpeed] = ConfigModule.Instance.Common().default_puppet_speed;
        _baseProps[ZoneProperty.InstantService] = 0;
        _baseProps[ZoneProperty.Product1Sub] = 0;
        _baseProps[ZoneProperty.Product2Sub] = 0;
        _baseProps[ZoneProperty.Product3Sub] = 0;
        _baseProps[ZoneProperty.WaitingTimeout] = _config.waiting_timeout;
        _baseProps[ZoneProperty.ExtraRevenueNail] = 0;
        _baseProps[ZoneProperty.ExtraRevenueHair] = 0;
        _baseProps[ZoneProperty.ExtraRevenueSpa] = 0;
        _baseProps[ZoneProperty.RevenueDouble] = 0;
        _baseProps[ZoneProperty.RevenueTriple] = 0;
        _baseProps[ZoneProperty.NoProductRequire] = 0;
        _baseProps[ZoneProperty.GuestRate] = 0;
        _baseProps[ZoneProperty.GuestInterval] = 0;
    }
    private void InitZoneItems()
    {
        foreach(var item in _config.items) {
            var level = _archiveData.item_levels[item.id];
            ZoneItemData zoneItemData = new ZoneItemData(this, item, level);
            _zoneItemDatas.Add(item.id, zoneItemData);
            _zoneProps[zoneItemData.Property] += zoneItemData.Value;
        }
    }
    private void RefreshStatus()
    {
        if(_archiveData.level == 0) {
            if(_team != null) _status = ZoneStatus.Levelup;
            else _status = ZoneStatus.Unbuilt;
        } else {
            if(_team != null) _status = ZoneStatus.Levelup;
            else _status = ZoneStatus.Built;
        }
    }
    private void RefreshStaffProps()
    {
        var propTypes = ZoneModule.Instance.ZonePropTypes;
        foreach(var propType in propTypes) {
            _staffProps[propType] = 0;
        }
        var staffData = StaffModule.Instance.GetArchiveData(_archiveData.staff);
        if(staffData != null) {
            foreach(var staffAttr in staffData.attributes) {
                var attr = ConfigModule.Instance.GetAttribute(staffAttr.attributeId);
                if(attr.global < 1 && (attr.service == _config.service || string.IsNullOrEmpty(attr.service))) {
                    _staffProps[attr.enhance_effect] += staffAttr.value;
                }
            }
        }
    }
    private void RefreshZoneProps(string prop, float oldValue, float newValue)
    {
        _zoneProps[prop] += newValue - oldValue;
    }
    public float GetProp(string prop, float extra = 0)
    {
        var func = prop[0];
        var talentRatio = string.Format("{0}_{1}", prop, _config.service);
        var talentValue = string.Format("{0}_{1}", prop, _config.service);

        var adBuffRatio = string.Format("{0}_{1}", prop, "ad_ratio");
        var shopBuffRatio = string.Format("{0}_{1}", prop, "shop_ratio");
        if(func == 'r') return _baseProps[prop] * (1 + _zoneProps[prop] + extra) * (1 + _staffProps[prop] + TalentModule.Instance.GetEnhanceValue(talentRatio) + BuffModule.Instance.GetBuff(adBuffRatio) + BuffModule.Instance.GetBuff(shopBuffRatio));
        else if(func == 'v') return _baseProps[prop] + _zoneProps[prop] + extra + _staffProps[prop] + TalentModule.Instance.GetEnhanceValue(talentValue);
        return 0;
    }
    private float GetNextProp(string itemId, string prop)
    {
        float extra = 0;
        if(_zoneItemDatas.TryGetValue(itemId, out var itemData)) {
            extra = itemData.NextValue - itemData.Value;
        }
        return GetProp(prop, extra);
    }
    public float ItemLevelRatio()
    {
        int totalLevel = 0;
        int curLevel = 0;
        foreach(var itemData in _zoneItemDatas.Values) {
            totalLevel += itemData.TotalLevelRato();
            curLevel += itemData.CurLevelRatio();
        }
        return totalLevel == 0 ? 0 : (float)curLevel / totalLevel;
    }
    public bool IsItemCurLevelMax()
    {
        foreach(var itemData in _zoneItemDatas.Values) {
            if(!itemData.IsLevelRatioMax()) return false;
        }
        return true;
    }
    public MillstoneBonus GetBonus()
    {
        if(_archiveData.level == 0) return null;
        return _config.bonus[_archiveData.level - 1];
    }
    public int GetProfit()
    {
        if(_archiveData.level <= 0) return 0;
        return (int)(GetProp(ZoneProperty.Revenue) * GetProp(ZoneProperty.ServiceEffy));
    }
    public bool LevelupZoneItem(string itemId)
    {
        if(_zoneItemDatas.TryGetValue(itemId, out var itemData)) {
            if(itemData.IsMax()) return false;
            if(PlayerModule.Instance.UseMoney(itemData.LvlupCost())) {
                var oldRatio = ItemLevelRatio();
                RefreshZoneProps(itemData.Property, itemData.Value, itemData.NextValue);
                itemData.Upgrade(_archiveData.LevelupZoneItem(itemId));
                var newRatio = ItemLevelRatio();
                MillstoneBonus bonus = GetBonus();
                if(bonus != null) {
                    if(oldRatio < 0.3 && newRatio >= 0.3) AddMillstoneBonus(bonus.item1, bonus.amount1);
                    if(oldRatio < 0.6 && newRatio >= 0.6) AddMillstoneBonus(bonus.item2, bonus.amount2);
                    if(oldRatio < 1 && newRatio >= 1) AddMillstoneBonus(bonus.item3, bonus.amount3);
                }
                return true;
            }
        }
        return false;
    }
    private void AddMillstoneBonus(string itemType, int number)
    {
        if(itemType == ItemType.Money) {
            PlayerModule.Instance.AddMoneyWithBigEffect(number, true);
        } else if(itemType == ItemType.Coin) {
            PlayerModule.Instance.AddCoinWithBigEffect(number, true);
        }
        PlayerModule.Instance.AddStar(1, true);
    }
    public bool FinishLevelup()
    {
        _archiveData.level++;
        // 自动worker
        // if(_config.type == BuidlingType.Cell) {
        //     _archive.staff = Config.id + "_worker";
        // }
        _team = null;
        TraceModule.Instance.TraceBuild(_config.id, _archiveData.level);
        RefreshStatus();
        RefreshBaseProps();
        return true;
    }
    public ZoneItemData GetItemData(string itemId)
    {
        return _zoneItemDatas.TryGetValue(itemId, out var itemData) ? itemData : null;
    }
    public bool ZoneRequire()
    {
        var level = _archiveData.level;
        if(level >= _config.max_level) return false;
        if(_config.zone_require.Count <= 0) return true;
        return ZoneModule.Instance.IsZoneLevelFit(_config.zone_require[level]);
    }
    public long GetWorkTeamRemainTime()
    {
        return _team != null ? _team.GetRemainTime() : 0;
    }
    public long GetWorkTeamRemainMilliTime()
    {
        return _team != null ? _team.GetRemainMilliTime() : 0;
    }
    public long GetWorkTeamDuration()
    {
        return _team != null ? _team.duration : 0;
    }
    #region ITimeProgressBarData
    public void Start(WorkTeamData team)
    {
        _team = team;
        RefreshStatus();
    }
    public void Finish()
    {
        ZoneModule.Instance.FinishLevelupZone(_config.id);
    }
    public void ReduceTimeByAd(long time, long adCooldown)
    {
        if (_team != null) _team.adCooldownTime = NtpModule.Instance.UtcNowSeconds + adCooldown;
        ZoneModule.Instance.ReduceTimeByAd(time, _config.id);
    }
    public string GetAdUnitName()
    {
        return AdUnitName.ad_unit_reward_building;
    }
    #endregion
}
