using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lemegeton;

public class WorkTeamData
{
    public string id;
    public string owner;
    public long endTime;
    public long duration;
    public int adCount;
    public long adCooldownTime;
    public WorkTeamData() {}

    public WorkTeamData(string id)
    {
        this.id = id;
        this.owner = string.Empty;
        this.endTime = -1;
        this.duration = 0;
        adCooldownTime = 0;
        adCount = ConfigModule.Instance.Common().ad_max;
    }

    public void Begin(string owner, long duration)
    {
        this.owner = owner;
        this.endTime = NtpModule.Instance.UtcNowSeconds + duration;
        this.duration = duration;
        this.adCooldownTime = 0;
        adCount = ConfigModule.Instance.Common().ad_max;
    }
    public void ReduceAd()
    {
        if(adCount > 0) adCount--;
    }
    public void Done()
    {
        this.owner = string.Empty;
        this.endTime = -1;
        this.duration = 0;
        this.adCooldownTime = 0;
        adCount = ConfigModule.Instance.Common().ad_max;
    }
    public void ResetAdCount()
    {
        adCount = ConfigModule.Instance.Common().ad_max;
    }
    public long GetRemainTime()
    {
        return endTime > 0 ? endTime - NtpModule.Instance.UtcNowSeconds : 0;
    }
    public long GetRemainMilliTime()
    {
        return endTime > 0 ? endTime * 1000 - NtpModule.Instance.UtcNowMillSeconds : 0;
    }
}