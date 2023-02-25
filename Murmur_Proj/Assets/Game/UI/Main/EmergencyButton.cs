using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;
using DG.Tweening;

public class EmergencyButton : MonoBehaviour
{
    [SerializeField] GameObject _normalContent;
    [SerializeField] GameObject _normalMark;
    [SerializeField] Text _normalTimerTxt;
    [SerializeField] GameObject _claimContent;
    [SerializeField] Button _emergencyBtn;
    private bool _shown;
    public void Start()
    {
        _shown = false;
        _emergencyBtn.onClick.AddListener(OnEmergencyBtn);
        EventModule.Instance.Register(EventDefine.EmergencyTimer, OnEmergencyTimerEvent);
        EventModule.Instance.Register(EventDefine.EmergencyUpdate, OnEmergencyUpdateEvent);
    }
    void OnDestroy()
    {
        EventModule.Instance?.UnRegister(EventDefine.EmergencyTimer, OnEmergencyTimerEvent);
        EventModule.Instance?.UnRegister(EventDefine.EmergencyUpdate, OnEmergencyUpdateEvent);
    }
    private void OnEmergencyTimerEvent(Component sender, EventArgs args)
    {
        EmergencyTimerArgs arg = args as EmergencyTimerArgs;
        var _emergencyData = EmergencyModule.Instance.CurEmergency;
        if(_emergencyData == null) return;

        if(_emergencyData.state == EmergencyState.Ready) {
            _normalTimerTxt.text = FormatUtil.FormatTimeShort(arg.timer);
        } else if(_emergencyData.state == EmergencyState.Running) {
            _normalTimerTxt.text = FormatUtil.FormatTimeShort(arg.timer);
        }
    }
    private void OnEmergencyUpdateEvent(Component sender, EventArgs args)
    {
        var _emergencyData = EmergencyModule.Instance.CurEmergency;
        if(_emergencyData == null) {
            if(_shown) {
                transform.DOLocalMoveX(-340f, 0.5f);
                _shown = false;
            }
        } else {
            if(_emergencyData.state == EmergencyState.Ready) {
                _normalContent.SetActive(true);
                _normalMark.SetActive(true);
                _normalTimerTxt.text = FormatUtil.FormatTimeShort(_emergencyData.config.timeout);
                _claimContent.SetActive(false);
                if(!_shown) {
                    _shown = true;
                    transform.DOLocalMoveX(0f, 0.5f);
                }
            } else if(_emergencyData.state == EmergencyState.Running) {
                _normalContent.SetActive(true);
                _normalMark.SetActive(false);
                _normalTimerTxt.text = FormatUtil.FormatTimeShort(_emergencyData.config.duration);
                _claimContent.SetActive(false);
            } else if(_emergencyData.state == EmergencyState.Done) {
                _normalContent.SetActive(false);
                _claimContent.SetActive(true);
            }
        }
    }

    private void OnEmergencyBtn()
    {
        UIMgr.Instance.OpenUIByClick(EmergencyView.PrefabPath, null, false, false);
    }
}
