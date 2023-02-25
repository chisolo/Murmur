using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonEx : Button
{

    public ButtonClickedEvent OnLongPress{
        get {
            return _onLongPress;
        }
        set {
            _onLongPress = value;
        }
    }
    public ButtonClickedEvent _onLongPress;
    private bool _startPress = false;
    private float _curPointDownTime = 0f;
    private float _longPressTime = 0.6f;
    private bool _longPressTrigger = false;
    protected ButtonEx()
    {
        _onLongPress = new ButtonClickedEvent();
    }


    void Update()
    {
        LongPress();
    }

    private void LongPress()
    {
        if(_startPress && !_longPressTrigger) {
            if(Time.time > _curPointDownTime + _longPressTime) {
                _longPressTrigger = true;
                _startPress = false;
                _onLongPress?.Invoke();
            }
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        _curPointDownTime = Time.time;
        _startPress = true;
        _longPressTrigger = false;
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        _startPress = false;
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        _startPress = false;
    }
}
