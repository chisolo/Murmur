using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Lemegeton;

public class OperateAction : PuppetAction
{
    private string _action;
    private float _duration;
    private bool _progress;
    private long _timerId;
    public static OperateAction Get(PuppetCtrl owner, string act, float duration, Action<PuppetCtrl> callback, bool progress)
    {
        OperateAction action = GenericPool<OperateAction>.Get();
        action.owner = owner;
        action.finish = false;
        action.callback = callback;
        action._action = act;
        action._duration = duration;
        action._progress = progress;
        action._timerId = -1;
        return action;
    }
    public override void Execute(PuppetCtrl puppetCtrl) 
    {
        base.Execute(puppetCtrl);
        if(!string.IsNullOrEmpty(_action)) {
            owner.Animator.SetBool(_action, true);
        }
        if(_duration < 0) return;
        if(_progress) {
            owner.ProgressView?.Show();
        }
        _timerId = TimerModule.Instance.CreateFrameTimer(_duration, OnComplete, false, OnRefresh);
    }
    public override void Finish()
    {
        if(callback != null) callback.Invoke(owner);
        Cancel();
    }
    public override void Cancel()
    {
        if(!string.IsNullOrEmpty(_action)) {
            owner.Animator.SetBool(_action, false);
        }
        TimerModule.Instance.CancelTimer(_timerId);
        _timerId = -1;
        if(_progress) {
            owner.ProgressView?.Hide();
        }
        base.Cancel();
        GenericPool<OperateAction>.Release(this);
    }
    private void OnComplete()
    {
        finish = true;
    }
    private void OnRefresh(float left)
    {
        if(!_progress) return;
        owner.ProgressView?.SetProgress( 1 - left / _duration);
    }
}
