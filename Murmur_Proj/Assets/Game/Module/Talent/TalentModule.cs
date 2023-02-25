using System.Collections.Generic;
using System.Linq;
using Lemegeton;
using UnityEngine;

public class TalentModule : Singleton<TalentModule>, ITalentApply
{
    protected TalentModule() { }
    private bool _inited = false;

    private Dictionary<string, TalentConfig> _talentConfigs;
    private Dictionary<string, TalentData> _talents;
    private Dictionary<string, float> _talentVals;
    private List<TalentData> _talentList;
    private CommonConfig _commonConfig;
    private TalentArchive _talentArchive;
    private WorkTeam _workTeam;
    public WorkTeam WorkTeam => _workTeam;

    public void Init()
    {
        if(_inited) return;

        _talentArchive = ArchiveModule.Instance.GetArchive<TalentArchive>();
        _talentConfigs = new Dictionary<string, TalentConfig>();
        _talents = new Dictionary<string, TalentData>();
        _talentList = new List<TalentData>();
        _talentVals = new Dictionary<string, float>();

        InitTalentDatas();
        SetTalentValue();

        foreach (var talentData in _talentList) {
            talentData.UpdateCost();
            talentData.UpdateStatus();
        }

        _inited = true;

        TimerModule.Instance.CreateFrameTimer(0.4f, () => {
            _workTeam.Process();
            foreach (var talentData in _talentList) {
                talentData.UpdateStatus();
            }
        }, loop: -1);
    }
    private void InitTalentDatas()
    {
        var talentConfigs = ConfigModule.Instance.Talents();
        var now = NtpModule.Instance.UtcNowSeconds;
        bool dirty = false;

        foreach(var config in talentConfigs) {
            var id = config.id;
            var archiveData = _talentArchive.GetData(id);
            if (archiveData == null) {
                archiveData = new TalentArchiveData(id);
                _talentArchive.AddData(id, archiveData);
                dirty = true;
            }
            var talentData = new TalentData();
            talentData.Init(archiveData, config);
            _talents.Add(config.id, talentData);
            _talentList.Add(talentData);

            _talentConfigs[id] = config;
        }
        if(_talentArchive.work_team.Capacity <= 0) {
            dirty = true;
            _talentArchive.work_team.AddTeam(GameUtil.DefaultTalentTeam);
        }
        _workTeam = _talentArchive.work_team;
        _workTeam.SetComplete(CompleteResearch);

        foreach(var team in _workTeam.teams) {
            if(!string.IsNullOrEmpty(team.owner) && team.endTime > 0) {
                if(team.endTime <= now) {
                    _talents[team.owner].Complete();
                    team.Done();
                    dirty = true;
                } else {
                    _talents[team.owner].Start(team);
                }
            }
        }

        if(dirty) {
            Save();
        }
    }

    private void SetTalentValue()
    {
        _talentVals.Clear();

        foreach (var talentData in _talentList) {
            var type = talentData.EnhanceType;
            if (!_talentVals.ContainsKey(type)) {
                _talentVals[type] = 0;
            }
            if (talentData.IsComplete) {
                _talentVals[type] += talentData.EnhanceValue;
            }
        }
    }

    public void Save()
    {
        ArchiveModule.Instance.SaveArchive(_talentArchive);
    }

    // void Update()
    // {
    //     if (!_inited) return;
    //     if(timer < 1f) {
    //         timer += Time.deltaTime;
    //         return;
    //     }

    //     timer = 0;
    //     // _workTeam.Process();
    //     // foreach (var talentData in _talentList) {
    //     //     talentData.UpdateStatus();
    //     // }
    // }

    public List<TalentData> GetTalentDatas() => _talentList;

    public bool CanResearch(TalentData talentData)
    {
        bool enoughMoney = PlayerModule.Instance.Money >= talentData.Cost;

        if (string.IsNullOrEmpty(talentData.Cfg.require_id)) {
            return enoughMoney;
        }

        return enoughMoney && IsComplete(talentData.Cfg.require_id);
    }

    public TalentData GetTalentData(string id)
    {
        return _talents.TryGetValue(id, out var value) ? value : null;
    }

    public string GetTalentName(string id)
    {
        if(_talentConfigs.TryGetValue(id, out var talentConfig)) {
            return talentConfig.name;
        }
        return string.Empty;
    }

    public float GetEnhanceValue(string enhance_type)
    {
        var val = 0f;
        _talentVals.TryGetValue(enhance_type, out val);
        return val;
    }
    public bool IsComplete(string id)
    {
        var talent = GetTalentData(id);
        if(talent == null) return false;
        return talent.IsComplete;
    }

    public bool IsCompleteByType(string enhance_type)
    {
        foreach (var talentData in _talentList) {
            if (talentData.EnhanceType == enhance_type && talentData.IsComplete) {
                return true;
            }
        }
        return false;
    }

    public void CompleteResearch(string talentId)
    {
        if(_talents.TryGetValue(talentId, out var talentData)) {
            CompleteResearch(talentData);
        }
    }
    public void CompleteResearch(TalentData talentData)
    {
        TraceModule.Instance.TraceTalent(talentData.Id);
        talentData.Complete();
        _workTeam.WorkDone(talentData.Id);

        //SetTalentValue();
        _talentVals[talentData.EnhanceType] += talentData.EnhanceValue;

        var target = TalentEnhanecType.GetApplyTarget(talentData.EnhanceType);
        if (target != null) {
            target.ApplyNewTalent(talentData.Id, talentData.EnhanceType, talentData.EnhanceValue);
        }

        using (var arg  = TalentResearchEventArgs.Get()) {
            arg.talentId = talentData.Id;
            arg.enhanceType = talentData.EnhanceType;
            EventModule.Instance.FireEvent(EventDefine.TalentResearchComplete, arg);
        }

        using (var arg  = EmptyEventArgs.Get()) {
            EventModule.Instance.FireEvent(EventDefine.TalentTeamUpdate, arg);
        }

        Save();
    }
    public void ReduceAdCount(TalentData talentData)
    {
        WorkTeamData team = _workTeam.GetTeamData(talentData.Id);
        if(team != null) {
            team.ReduceAd();
            Save();
        }
    }
    public void StartResearch(TalentData talentData)
    {
        if(PlayerModule.Instance.UseMoney(talentData.Cost)) {
            var team = _workTeam.GetFreeTeam();
            if(team == null) return;
            team.Begin(talentData.Id, talentData.Cfg.time);
            talentData.Start(team);

            using (var arg  = TalentResearchEventArgs.Get()) {
                arg.talentId = talentData.Id;
                arg.enhanceType = talentData.EnhanceType;
                EventModule.Instance.FireEvent(EventDefine.TalentResearchStart, arg);
            }

            using (var arg  = EmptyEventArgs.Get()) {
                EventModule.Instance.FireEvent(EventDefine.TalentTeamUpdate, arg);
            }

            talentData.UpdateStatus();
        }
        Save();
    }

    public void ReduceTime(TalentData talentData, long time)
    {
        talentData.ReduceTime(time);
        Save();
    }

    public string FindNotOwnTeam()
    {
        return GameUtil.TalentTeamIdList.FirstOrDefault(id => !_workTeam.teams.Any(team => team.id == id));
    }

    public void AddTeam()
    {
        if (_workTeam.IsTeamMax()) {
            return;
        }

        var teamId = FindNotOwnTeam();
        Debug.Log("AddTeam: " + teamId);
        _workTeam.AddTeam(teamId);

        using (var arg  = EmptyEventArgs.Get()) {
            EventModule.Instance.FireEvent(EventDefine.TalentTeamUpdate, arg);
        }

        Save();
    }

    public void AddTeam(string teamId)
    {
        _workTeam.AddTeam(teamId);
        using (var arg  = EmptyEventArgs.Get()) {
            EventModule.Instance.FireEvent(EventDefine.TalentTeamUpdate, arg);
        }
        Save();
    }

    public TalentData GetProcessTalentData()
    {
        long min = long.MaxValue;
        string talentId = string.Empty;

        foreach (var team in _workTeam.teams) {
            if (team.endTime > 0 && team.endTime < min) {
                talentId = team.owner;
                min = team.endTime;
            }
        }

        return GetTalentData(talentId);
    }

    public void ApplyNewTalent(string id, string type, float value)
    {
        foreach (var talentData in _talentList) {
            talentData.UpdateCost();
        }
    }

    // public ITalentApply GetApplyTarget2(string enhanceType) => enhanceType switch
    //     {
    //         "b" => null,
    //         "a" => null,
    //         _ => null,
    //     };

    public bool IsTalentReady(string talentId)
    {
        if(string.IsNullOrEmpty(talentId)) return true;
        var talent = GetTalentData(talentId);
        return talent.IsComplete;
    }

#if UNITY_EDITOR
    // TODO: debug mode in mobile
    [UnityEditor.MenuItem("Dev/Talent/resetAd")]
    public static void ResetAd_Menu()
    {
        foreach (var team in Instance._workTeam.teams) {
            team.ResetAdCount();
        }
    }

    [UnityEditor.MenuItem("Dev/Talent/AddTalentTeam")]
    public static void AddTalentTeam_Menu()
    {
        TalentModule.Instance.AddTeam();
    }

#endif
}