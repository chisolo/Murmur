using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lemegeton;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;
public class IncomeBoostPopup : PopupUIBaseCtrl
{
    public static string PrefabPath = "Assets/Res/UI/Prefab/MainPopup/IncomeBoostPopup.prefab";

    public static void Open()
    {
        var arg = PopupUIArgs.Empty;
        UIMgr.Instance.OpenUIByClick(PrefabPath, arg, false, true);
    }

    [SerializeField] Text _boostDesc;
    [SerializeField] Text _boostRatio;
    [SerializeField] Text _boostLimit;
    [SerializeField] Text _boostBtnTime;
    [SerializeField] Text _remainTime;
    [SerializeField] Image _front;
    [SerializeField] Button _boostBtn;
    [SerializeField] GameObject _progress;

    private long _timerId = -1;

    public override void Init(PopupUIArgs arg)
    {
        var config = ConfigModule.Instance.Common();
        var limit = config.ad_income_boost_limit_time * 60;

        _boostRatio.text = string.Format("+{0:P0}", config.ad_income_ratio);
        _boostLimit.text = FormatUtil.FormatTimeAuto(limit);
        _boostDesc.text = string.Format("{0} <color=#10EF82>+{1:P0}</color>", "INCOME_BOOST_QUICK_TRAINING_DESC".Locale(), config.ad_income_ratio);
        _boostBtnTime.text = string.Format("+{0}min", config.ad_income_boost_time);

        UpdateProgressTime();
        _timerId = TimerModule.Instance.CreateTimer(1, () => {
            UpdateProgressTime();
        }, loop: -1);
    }

    void UpdateProgressTime()
    {
        var config = ConfigModule.Instance.Common();
        var limit = config.ad_income_boost_limit_time * 60;
        var now = NtpModule.Instance.UtcNowSeconds;
        if (BuffModule.Instance.adIncomeBoostEndTime > now) {
            var remain = BuffModule.Instance.adIncomeBoostEndTime - now;
            _remainTime.text = FormatUtil.FormatTimeAuto(remain);
            _progress.gameObject.SetActiveIfNeed(true);
            _front.fillAmount = (float)remain/ limit;
        } else {
            _progress.gameObject.SetActiveIfNeed(false);
        }
    }

    void OnDestroy()
    {
        TimerModule.Instance?.CancelTimer(_timerId);
    }

    public void OnClickShopBtn()
    {
        this.Hide();
        AppLogger.Log("OnClickShopBtn");
        var id = ConfigModule.Instance.Common().ad_income_upgrade_treasury_id;
        ShopUICtrl.Open(id);
    }

    public void OnClickAdBtn()
    {
        WatchAd();
    }

    private void WatchAd()
    {
        AdModule.Instance.ShowRewardAd(AdUnitName.ad_unit_reward_income_boost, () => {
            BuffModule.Instance.AddIncomeBoostBuff();
            UpdateProgressTime();
        });
    }

}