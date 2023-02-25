using Lemegeton;
using UnityEngine;

public class TalentData: ITimeProgressBarData
{
    private TalentConfig _config;
    private TalentArchiveData _archive;
    private TalentStatus _status;
    private WorkTeamData _team;
    private int _cost;
    public void Init(TalentArchiveData archive, TalentConfig config)
    {
        _config = config;
        _archive = archive;
        _team = null;

        _cost = config.cost;
    }
    public TalentArchiveData Archive => _archive;
    public WorkTeamData Team => _team;
    public bool IsComplete => _archive.completed;
    public long EndTime => Team?.endTime ?? -1;
    public string Id => _archive.id;

    public TalentConfig Cfg => _config;
    public float EnhanceValue => _config.value;
    public string EnhanceType => _config.enhance_type;

    public long RemainTime => EndTime - NtpModule.Instance.UtcNowSeconds;
    public long Duration => _config.time;
    public long AdCooldownTime => Team?.adCooldownTime ?? 0;

    public int Cost => _cost;

    public TalentStatus Status => _status;

    public int AdCount => Team?.adCount ?? 0;
    public void Start(WorkTeamData team)
    {
        _team = team;
    }

    public void Complete()
    {
        _archive.completed = true;
        _team = null;
    }

    public void ReduceTime(long time)
    {
        if (Team != null) {
            Team.endTime -= time;
        }
    }

    public TalentStatus GetStatus()
    {
        if (IsComplete) {
            // 研究完成
            return TalentStatus.Complete;
        } else if (EndTime > 0) {
            // 研究中
            return TalentStatus.InProgress;

        } else if (TalentModule.Instance.CanResearch(this)) {
            // 可研究
            return TalentStatus.CanResearch;
        } else {
            // 不可研究
            return TalentStatus.CannotResearch;
        }
    }

    public void UpdateStatus()
    {
        _status = GetStatus();
    }

    public void Finish()
    {
        TalentModule.Instance.CompleteResearch(this);
    }

    public void ReduceTimeByAd(long time, long adCooldown)
    {
        if (_team != null) _team.adCooldownTime = NtpModule.Instance.UtcNowSeconds + adCooldown;
        TalentModule.Instance.ReduceTime(this, time);
        TalentModule.Instance.ReduceAdCount(this);

    }

    public void UpdateCost()
    {
        _cost = Mathf.FloorToInt(_config.cost * (1f - TalentModule.Instance.GetEnhanceValue(TalentEnhanecType.DISCOUNT_TALENT)));
    }

    public string GetAdUnitName()
    {
        return AdUnitName.ad_unit_reward_research;
    }
}