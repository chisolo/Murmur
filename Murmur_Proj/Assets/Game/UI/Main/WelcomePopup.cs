using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lemegeton;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;
using System;

public class WelcomePopup : PopupUIBaseCtrl
{
    public static string PrefabPath = "Assets/Res/UI/Prefab/MainPopup/WelcomePopup.prefab";

    public class Args : PopupUIArgs
    {
        public int number;
    }

    public static void Open(Action onEnd = null)
    {
        if (PlayerModule.Instance.LastPlayTime == 0) {
            onEnd?.Invoke();
            return;
        }

        var time = PlayerModule.Instance.OfflineTime;
        var show = ConfigModule.Instance.Common().offline_time;
        AppLogger.Log("WelcomePopup Open " + time);

        if (time <= show) {
            onEnd?.Invoke();
            return;
        }
        var num = time * BuildingModule.Instance.GetTotalProfit() * ConfigModule.Instance.Common().offline_income_factor;
        var get = (int)System.Math.Ceiling(num);

        //AppLogger.Log("WelcomePopup get " + get);
        if (get <= 0) {
            onEnd?.Invoke();
            return;
        }
        EventModule.Instance?.FireEvent(EventDefine.ForceCloseUI);
        var arg = new Args();
        arg.number = get;
        UIMgr.Instance.OpenUI(PrefabPath, arg, false, false, null, () => {
            onEnd?.Invoke();
        });
    }

    [SerializeField] Text _getNum;
    [SerializeField] Text _moneyText;
    [SerializeField] GameObject _noSpace;

    private int _get;

    public override void Init(PopupUIArgs arg)
    {
        var now = PlayerModule.Instance.Money;
        var limit = PlayerModule.Instance.MoneyLimit;

        var get = (arg as Args).number;
        get = Mathf.Min(get, limit);
        _get = get;

        var all = now + get;
        if (all >= limit) {
            _noSpace.SetActive(true);
        } else {
            _noSpace.SetActive(false);
        }

        _getNum.text = FormatUtil.Currency(get, false);
        _moneyText.text = string.Format("+ {0} / {1}", FormatUtil.Currency(now, false), FormatUtil.Currency(limit, false));
    }

    public void OnClickShopBtn()
    {
        AppLogger.Log("OnClickShopBtn");
        var id = ConfigModule.Instance.Common().welcome_upgrade_vault_id;

        ShopUICtrl.Open(id);
    }

    public void OnClickAdBtn()
    {
        WatchAd();
    }

    private void WatchAd()
    {
        AdModule.Instance.ShowRewardAd(AdUnitName.ad_unit_reward_welcome_double, () => {
            PlayerModule.Instance.AddMoneyWithBigEffect(_get * 2, true);
            this.Hide();
        });
    }

    public void OnClickCloseAndSendReward()
    {
        PlayerModule.Instance.AddMoneyWithBigEffect(_get, true);
        this.Hide();
    }

}
