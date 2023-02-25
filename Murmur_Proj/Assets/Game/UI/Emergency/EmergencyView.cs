using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;

public class EmergencyView : PopupUIBaseCtrl
{
    public static string PrefabPath = "Assets/Res/UI/Prefab/Emergency/EmergencyView.prefab";
    [SerializeField] Button _closeBtn;
    [SerializeField] GameObject _timerContent;
    [SerializeField] Text _timerText;
    [SerializeField] Text _bonusText;
    [SerializeField] GameObject _acceptContent;
    [SerializeField] Text _acceptTimerText;
    [SerializeField] Button _acceptBtn;
    [SerializeField] GameObject _doubleContent;
    [SerializeField] Button _doubleBtn;
    [SerializeField] GameObject _deliverContent;
    [SerializeField] Text _deliverTimerText;
    public override void Init(PopupUIArgs arg)
    {
        EventModule.Instance.Register(EventDefine.EmergencyTimer, OnEmergencyTimerEvent);
        EventModule.Instance.Register(EventDefine.EmergencyUpdate, OnEmergencyUpdateEvent);
    }
    void OnDestroy()
    {
        EventModule.Instance?.UnRegister(EventDefine.EmergencyTimer, OnEmergencyTimerEvent);
        EventModule.Instance?.UnRegister(EventDefine.EmergencyUpdate, OnEmergencyUpdateEvent);
    }
    void Start()
    {
        _closeBtn.onClick.AddListener(OnCloseBtn);
        _acceptBtn.onClick.AddListener(OnAcceptBtn);
        _doubleBtn.onClick.AddListener(OnDoubleBtn);
        Refresh();
    }

    private void Refresh()
    {
        var emergencyData = EmergencyModule.Instance.CurEmergency;
        if(emergencyData == null) {
            Hide();
            return;
        }
        _bonusText.text = FormatUtil.Revenue(emergencyData.bonus);
        if(emergencyData.state == EmergencyState.Ready) {
            _timerContent.SetActive(true);
            _acceptContent.SetActive(true);
            _doubleContent.SetActive(false);
            _deliverContent.SetActive(false);
            _timerText.text = FormatUtil.FormatTimeShort((long)emergencyData.config.duration);
            _acceptTimerText.text = FormatUtil.FormatTimeShort(EmergencyModule.Instance.LeftTime());
        } else if(emergencyData.state == EmergencyState.Running) {
            _timerContent.SetActive(false);
            _acceptContent.SetActive(false);
            _doubleContent.SetActive(false);
            _deliverContent.SetActive(true);
            _deliverTimerText.text = FormatUtil.FormatTimeShort(EmergencyModule.Instance.LeftTime());
        } else if(emergencyData.state == EmergencyState.Done) {
            _timerContent.SetActive(false);
            _acceptContent.SetActive(false);
            _doubleContent.SetActive(true);
            _deliverContent.SetActive(false);
        }
    }

    private void OnAcceptBtn()
    {
        Hide();
        EmergencyModule.Instance.AcceptEmergency();
    }

    private void OnDoubleBtn()
    {
        WatchAd();
    }

    private void WatchAd()
    {
        AdModule.Instance.ShowRewardAd(AdUnitName.ad_unit_reward_emergency_double, () => {
            EmergencyModule.Instance.ClaimEmergency(true);
            this.Hide();
        });
    }

    private void OnCloseBtn()
    {
        Hide();
        var emergencyData = EmergencyModule.Instance.CurEmergency;
        if(emergencyData.state == EmergencyState.Done) {
            EmergencyModule.Instance.ClaimEmergency(false);
        }
    }
    private void OnEmergencyTimerEvent(Component sender, EventArgs args)
    {
        var emergencyData = EmergencyModule.Instance.CurEmergency;
        if(emergencyData == null) return;
        EmergencyTimerArgs arg = args as EmergencyTimerArgs;
        if(emergencyData.state == EmergencyState.Ready) {
            _acceptTimerText.text = FormatUtil.FormatTimeShort(arg.timer);
        } else if(emergencyData.state == EmergencyState.Running) {
            _deliverTimerText.text = FormatUtil.FormatTimeShort(arg.timer);
        }
    }
    private void OnEmergencyUpdateEvent(Component sender, EventArgs args)
    {
        Refresh();
    }
}
