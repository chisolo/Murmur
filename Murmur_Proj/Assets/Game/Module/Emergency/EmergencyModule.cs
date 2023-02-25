using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Lemegeton;

public enum EmergencyState
{
    Ready, // 可以开始
    Running,//　执行中倒计时
    Done, // 完成
}
public class EmergencyModule : Singleton<EmergencyModule>
{
    EmergencyModule() {}
    public float Interval => _emergencyConfig.interval;
    public EmergencyData CurEmergency => _curEmergency;
    private EmergencyConfig _emergencyConfig;
    private EmergencyData _curEmergency;
    private long _timeoutTimer = -1;
    private long _runTimer = -1;
    public void Init()
    {
        _emergencyConfig = ConfigModule.Instance.Emergency();
        _curEmergency = null;
    }
    public void OnLoad()
    {
        StartCoroutine(EmergencyCoroutine());
    }
    public IEnumerator EmergencyCoroutine()
    {
        while(true) {
            yield return new WaitForSeconds(EmergencyModule.Instance.Interval);
            if(_curEmergency == null && PlayerModule.Instance.Star >= _emergencyConfig.star) {
                var index = RandUtil.Range(_emergencyConfig.emergencies.Count);
                _curEmergency = new EmergencyData(_emergencyConfig.emergencies[index]);
                _timeoutTimer = TimerModule.Instance.CreateTimer(_curEmergency.config.timeout, OnTimeoutComplete, false, OnTimeoutStep);
                EventModule.Instance.FireEvent(EventDefine.EmergencyUpdate);
            }
        }
    }
    private void OnTimeoutComplete()
    {
        _curEmergency = null;
        TimerModule.Instance.CancelTimer(_timeoutTimer);
        _timeoutTimer = -1;
        EventModule.Instance.FireEvent(EventDefine.EmergencyUpdate);
    }

    private void OnTimeoutStep(float timeout)
    {
        using(EmergencyTimerArgs args = EmergencyTimerArgs.Get()) {
            args.timer = (long)(timeout + 0.5);
            EventModule.Instance.FireEvent(EventDefine.EmergencyTimer, args);
        }
    }
    public void AcceptEmergency()
    {
        if(_curEmergency != null) {
            _curEmergency.state = EmergencyState.Running;
            TimerModule.Instance.CancelTimer(_timeoutTimer);
            _timeoutTimer = -1;
            _runTimer = TimerModule.Instance.CreateTimer(_curEmergency.config.timeout, OnRunComplete, false, OnRunStep);
            EventModule.Instance.FireEvent(EventDefine.EmergencyUpdate);
        }
    }
    private void OnRunComplete()
    {
        CompleteEmergency();
    }
    private void OnRunStep(float timeout)
    {
        using(EmergencyTimerArgs args = EmergencyTimerArgs.Get()) {
            args.timer = (long)(timeout + 0.5);
            EventModule.Instance.FireEvent(EventDefine.EmergencyTimer, args);
        }
    }
    public void CompleteEmergency()
    {
        if(_curEmergency != null) {
            TimerModule.Instance.CancelTimer(_runTimer);
            _runTimer = -1;
            _curEmergency.state = EmergencyState.Done;
            EventModule.Instance.FireEvent(EventDefine.EmergencyUpdate);
        }
    }
    public void ClaimEmergency(bool isDouble)
    {
        if(_curEmergency != null) {
            var revenue = isDouble ? _curEmergency.bonus * 2 : _curEmergency.bonus;
            PlayerModule.Instance.AddMoneyWithBigEffect(revenue, true);
            _curEmergency = null;
            EventModule.Instance.FireEvent(EventDefine.EmergencyUpdate);
        }
    }
    public long LeftTime()
    {
        if(_timeoutTimer != -1) return (long)(TimerModule.Instance.GetTimer(_timeoutTimer) + 0.5f);
        if(_runTimer != -1) return (long)(TimerModule.Instance.GetTimer(_runTimer) + 0.5f);
        return -1;
    }
}
