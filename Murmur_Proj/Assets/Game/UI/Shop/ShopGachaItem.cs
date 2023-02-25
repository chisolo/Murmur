using System;
using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class ShopGachaItem : MonoBehaviour
{
    [SerializeField] Button _buyBtn;
    [SerializeField] Text _costText;
    [SerializeField] Button _adBtn;
    [SerializeField] Text _adFreeTime;
    [SerializeField] GameObject _adFreeRoot;

    [SerializeField] GameObject _gachaReddot;
    [SerializeField] GameObject _gachaReddot2;

    private ShopGachaConfig _gacha;
    private bool _isBasic;
    private long _timerId = -1;

    void Awake()
    {
        _buyBtn.onClick.AddListener(OnClickGachaBtn);
        if (_adBtn != null) {
            _adBtn.onClick.AddListener(OnClickAdBtn);
        }
    }

    public void Init(ShopGachaConfig gacha, bool isBasic)
    {
        _gacha = gacha;
        _isBasic = isBasic;

        _costText.text = gacha.cost.ToString();

        if (isBasic) {
            var adValid = ShopModule.Instance.IsBasicGachaAdValid();
            SetupBasic(adValid);
        }
    }

    void OnDestroy()
    {
        TimerModule.Instance?.CancelTimer(_timerId);
    }

    void SetupBasicBtn(bool adValid)
    {
        _adBtn.gameObject.SetActive(adValid);
        _buyBtn.gameObject.SetActive(!adValid);
        _adFreeRoot.SetActive(!adValid);

        _gachaReddot.SetActive(ShopModule.Instance.IsShowFreeGacheReddot());
        _gachaReddot2.SetActive(ShopModule.Instance.IsShowFreeGacheReddot());

        if (!adValid) {
            var remain = ShopModule.Instance.BasicValidReaminTime();
            _adFreeTime.text = FormatUtil.FormatTimeAuto(remain);
        }
    }

    void SetupBasic(bool adValid)
    {
        SetupBasicBtn(adValid);

        if (!adValid) {
            _timerId = TimerModule.Instance.CreateTimer(ShopModule.Instance.BasicValidReaminTime(), () => {
                SetupBasicBtn(true);
            }, false, (x) => {
                var remain = ShopModule.Instance.BasicValidReaminTime();
                _adFreeTime.text = FormatUtil.FormatTimeAuto(remain);
            });
        }
    }

    public void OnClickGachaBtn()
    {
        AppLogger.Log("Staff Gacha");
        string id = "";
        if (_gacha.cost_type == ItemType.Coin) {
            if (PlayerModule.Instance.UseCoin(_gacha.cost)) {
                id = StaffModule.Instance.DrawStaff(_gacha.group);
            } else {
                UIMgr.Instance.OpenNotEnoughCoin();
            }
        } else if (_gacha.cost_type == ItemType.Coupon) {
            if (PlayerModule.Instance.UseCoupon(_gacha.cost)) {
                id = StaffModule.Instance.DrawStaff(_gacha.group);
            }
        }

        if (!string.IsNullOrEmpty(id)) {
            ShowResult(id);
        }
    }

    public void OnClickAdBtn()
    {
        if (!_isBasic) return;

        WatchAd();
    }

    private void WatchAd()
    {
        AdModule.Instance.ShowRewardAd(AdUnitName.ad_unit_reward_gacha, () => {
            var id = StaffModule.Instance.DrawStaff(_gacha.group);
            ShopModule.Instance.UpdateBasicGachaValidTime();

            SetupBasic(false);
            if (!string.IsNullOrEmpty(id)) {
                ShowResult(id);
            }
        });
    }

    void ShowResult(string id)
    {
        ShopGachaResult.Open(id);
    }
}