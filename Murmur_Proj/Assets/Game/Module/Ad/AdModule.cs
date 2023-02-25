using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lemegeton;
using System.Threading.Tasks;

public class AdModule : Singleton<AdModule>
{

    public bool HasRewarded => _hasRewardAd;
    private bool _hasRewardAd = false;

    // 获取现金按钮，可显示时间
    public long ExtraCashValidTime = 0;
    public long ServiceBoostValidTime = 0;
    public long MoreGuestValidTime = 0;

    public bool hasShowPromotion;
    public bool canShowPromotion;

    public void Init()
    {
        hasShowPromotion = false;
        canShowPromotion = false;


    }

    public void OnLoad()
    {
        PreLoadAd();

        var config = ConfigModule.Instance.Common();

        TimerModule.Instance.CreateTimer(config.show_promotion_by_popup_time, () => {
            //Debug.Log("show_promotion_by_popup_time");
            canShowPromotion = true;
        });
    }

    private void PreLoadAd()
    {
#if !UNITY_EDITOR
        MaxSDK.Instance.RewardAdPreload("",
            (int ret, string msg, string data) =>  {
                //处理预加载后的逻辑
                _hasRewardAd = true;
            });
#else
        _hasRewardAd = true;
#endif

        TimerModule.Instance.CreateTimer(10f, () => {
            _hasRewardAd = true;
        });
    }

    public struct ShowAdResult
    {
        public int ret;
        public string msg;
        public string data;

        public ShowAdResult(int ret, string msg, string data)
        {
            this.ret = ret;
            this.msg = msg;
            this.data = data;
        }

        public bool IsSuccess => ret == 1;
    }

    public void ShowRewardAd(string pname, System.Action rewardAction)
    {
        if (ShopModule.Instance.IsVip) {
            rewardAction?.Invoke();
            return;
        }
#if !UNITY_EDITOR
        //UIMgr.Instance.ShowLoading(-1);
        MaxSDK.Instance.RewardAdShow(pname,
            (int ret, string msg, string data) =>
            {
                if (ret == 1) {
                    rewardAction?.Invoke();
                    PreLoadAd();
                    _hasRewardAd = true;
                }
            }
        );
#else
        UIMgr.Instance.ShowLoading(-1);
        TaskModule.Instance.Delay(1000, () => {
            rewardAction?.Invoke();
            UIMgr.Instance.HideLoading();
        });

#endif
    }

    // public Task<ShowAdResult> ShowRewardAdAsync(string pname)
    // {
    //     var task = new TaskCompletionSource<ShowAdResult>();

    //     ShowRewardAd(pname, (int ret, string msg, string data) =>  {
    //         var result = new ShowAdResult(ret, msg, data);
    //         task.SetResult(result);
    //     });

    //     return task.Task;
    // }
    public void AddExtraCashTime(int time)
    {
        ExtraCashValidTime = NtpModule.Instance.UtcNowSeconds + time;
    }

    public void AddServiceBoostValidTime(int time)
    {
        ServiceBoostValidTime = NtpModule.Instance.UtcNowSeconds + time;
    }

    public void AddMoreGuestValidTime(int time)
    {
        MoreGuestValidTime = NtpModule.Instance.UtcNowSeconds + time;
    }

    public long GetReduceTime(long remain)
    {
        if (remain <= 600) return 90;
        if (remain <= 1800) return 180;
        if (remain <= 3600) return 600;
        if (remain <= 7200) return 1200;
        if (remain <= 14400) return 2400;
        if (remain <= 21600) return 3600;

        return 4800;
    }

    public long GetCooldown(long remain)
    {
        if (remain <= 600) return 90;
        if (remain <= 1800) return 120;
        if (remain <= 3600) return 180;
        if (remain <= 7200) return 240;
        if (remain <= 14400) return 300;
        if (remain <= 21600) return 450;

        return 600;
    }


}
