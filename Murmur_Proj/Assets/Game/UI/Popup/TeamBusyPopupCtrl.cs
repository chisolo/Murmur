using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lemegeton;
using System;
using UnityEngine.UI;

public class TeamBusyPopupCtrl : PopupUIBaseCtrl
{
    public class Args : PopupUIArgs
    {
        public string title;
        public string body;
        public string shopText;
        public bool teamMax;

        public ITimeProgressBarData timeData;
    }

    public static string PrefabPath = "Assets/Res/UI/Prefab/Popup/TeamBusyPopup.prefab";

    [SerializeField]
    private Text _titleText;

    [SerializeField]
    private Text _bodyText;

    [SerializeField]
    private TimeProgressBar _timeProgress;

    [SerializeField]
    private Button _finishBtn;
    [SerializeField]
    private Text _finishCostTxt;

    [SerializeField]
    private Button _shopBtn;
    [SerializeField]
    private Text _shopText;

    private long _endTime => _timeData.EndTime;
    private long _duration => _timeData.Duration;

    private ITimeProgressBarData _timeData;

    public override void Init(PopupUIArgs arg)
    {
        var param = (Args)arg;

        _titleText.text = param.title;
        _bodyText.text = param.body;

        _timeData = param.timeData;

        if (_timeData == null) {
            Debug.LogError("[TeamBusyPopupCtrl] time data is null");
            HideForce();
            return;
        }

        var remain = _endTime - NtpModule.Instance.UtcNowSeconds;

        _shopBtn.onClick.RemoveAllListeners();
        _shopBtn.onClick.AddListener(OnClickShopBtn);
        _shopText.text = param.shopText;
        if (param.teamMax) {
            _shopBtn.gameObject.SetActive(false);
        }

        _finishCostTxt.text = FormatUtil.Currency(PlayerModule.Instance.TimeToGem(remain));
        _finishBtn.gameObject.SetActive(true);
        _finishBtn.interactable = true;

        _finishBtn.onClick.RemoveAllListeners();
        _finishBtn.onClick.AddListener(OnClickFinishBtn);

        _timeProgress.Init(_timeData, RefreshProgress, EndProgress);
    }


    private void RefreshProgress()
    {
        var remain = _endTime - NtpModule.Instance.UtcNowSeconds;
        _finishCostTxt.text = FormatUtil.Currency(PlayerModule.Instance.TimeToGem(remain));
    }
    private void EndProgress()
    {
        _shopBtn.interactable = false;
        _finishBtn.interactable = false;
        Hide();
    }

    private void OnClickShopBtn()
    {
        ShopUICtrl.Open(ShopUICtrl.Section.Builder);
        Hide();
    }

    private void OnClickFinishBtn()
    {
        var remain = _endTime - NtpModule.Instance.UtcNowSeconds;
        var cost = PlayerModule.Instance.TimeToGem(remain);
        if (PlayerModule.Instance.UseCoin(cost)) {
            _timeData.Finish();
        } else {
            Debug.Log("jump to shop");
            UIMgr.Instance.OpenNotEnoughCoin();
        }
        Hide();
    }

}