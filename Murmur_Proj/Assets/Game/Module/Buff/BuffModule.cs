using System.Collections.Generic;
using System.Linq;
using Lemegeton;
using UnityEngine;

public class BuffModule : Singleton<BuffModule>
{
    private bool _inited = false;
    private Dictionary<string, float> _bufftVals;
    private bool isInIncomeBoostBuff;

    public long adIncomeBoostEndTime => ShopModule.Instance.ShopArchive.adIncomeBoostEndTime;
    public long serviceBoostEndTime => ShopModule.Instance.ShopArchive.serviceBoostEndTime;

    public void Init()
    {
        if(_inited) return;
        isInIncomeBoostBuff = false;
        _bufftVals = new Dictionary<string, float>();

        UpdateIncomeBoostBuff();

        TimerModule.Instance.CreateTimer(1f, () => {
            UpdateIncomeBoostBuff();
        }, loop: -1);


        _bufftVals[BuffType.BuffShopIncomeType] = ShopModule.Instance.getPlayerRightValue(ShopRewardType.Income);

        InitServiceBoostBuff();

        _inited = true;
    }

    public float GetBuff(string type)
    {
        var val = 0f;
        _bufftVals.TryGetValue(type, out val);
        return val;
    }

    public void UpdateShopIncomeBuff(float val)
    {
        _bufftVals[BuffType.BuffShopIncomeType] = val;
        BuildingModule.Instance.RefreshBuildingProps();
    }

    void UpdateIncomeBoostBuff()
    {
        var adTime = adIncomeBoostEndTime;
        var now = NtpModule.Instance.UtcNowSeconds;

        if (adTime > now && !isInIncomeBoostBuff) {
            isInIncomeBoostBuff = true;
            _bufftVals[BuffType.BuffAdIncomeType] = ConfigModule.Instance.Common().ad_income_ratio;
        }

        else if (adTime <= now && isInIncomeBoostBuff) {
            isInIncomeBoostBuff = false;
            _bufftVals[BuffType.BuffAdIncomeType] = 0;

            BuildingModule.Instance.RefreshBuildingProps();
        }
    }

    public void AddIncomeBoostBuff()
    {
        var now = NtpModule.Instance.UtcNowSeconds;
        var boost_time = ConfigModule.Instance.Common().ad_income_boost_time * 60;
        if (adIncomeBoostEndTime > now) {
            var time = adIncomeBoostEndTime + boost_time;
            var remain = time - now;
            var max = ConfigModule.Instance.Common().ad_income_boost_limit_time * 60;
            remain = remain > max ? max : remain;
            time = now + remain;

            ShopModule.Instance.UpdateadIncomeBoostEndTime(time);
        } else {
            isInIncomeBoostBuff = true;
            _bufftVals[BuffType.BuffAdIncomeType] = ConfigModule.Instance.Common().ad_income_ratio;
            var time = now + boost_time;

            ShopModule.Instance.UpdateadIncomeBoostEndTime(time);

            BuildingModule.Instance.RefreshBuildingProps();
        }
    }

    public void InitServiceBoostBuff()
    {
        var end = serviceBoostEndTime;
        var now = NtpModule.Instance.UtcNowSeconds;

        if (end > now) {
            _bufftVals[BuffType.BuffServiceBoostType] = ConfigModule.Instance.ServiceBoost().effect_ratio;
        }
    }

    public void AddServiceBoostBuff()
    {
        var now = NtpModule.Instance.UtcNowSeconds;
        var config = ConfigModule.Instance.ServiceBoost();
        var boost_time = config.effect_time;

        _bufftVals[BuffType.BuffServiceBoostType] = config.effect_ratio;
        var time = now + boost_time;
        ShopModule.Instance.UpdateadServiceBoostEndTime(time);

        TimerModule.Instance.CreateTimer(boost_time, () => {
            AppLogger.Log("### boost end");
            _bufftVals[BuffType.BuffServiceBoostType] = 0;

            AdModule.Instance.AddServiceBoostValidTime(config.cooldown);
        });
    }

}
