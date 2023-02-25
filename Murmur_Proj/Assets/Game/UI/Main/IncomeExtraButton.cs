using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class IncomeExtraButton : MonoBehaviour
{
    [SerializeField]
    private Button _button;
    [SerializeField] Transition _trans;

    private long _timerId = -1;
    private bool active = false;

    void Start()
    {
        _button.onClick.AddListener(OnClickButton);

        _trans.Right();
        active = false;

        _timerId = TimerModule.Instance.CreateTimer(1f, () => {
            //Debug.Log("time " + NtpModule.Instance.UtcNowMillSeconds);
            UpdateTime();
        }, loop: -1);

    }

    void OnDestroy()
    {
        TimerModule.Instance?.CancelTimer(_timerId);
    }

    void UpdateTime()
    {
        var now = NtpModule.Instance.UtcNowSeconds;
        var validTime = AdModule.Instance.ExtraCashValidTime;

        if (AdModule.Instance.HasRewarded && now >= validTime && !active) {
            active = true;
            _trans.Back();
            return;
        }

        if (active && now < validTime) {
            active = false;
            _trans.Right();
        }
    }

    public void OnClickButton()
    {
        //IncomeExtraPopup.Open();

        var arg = PopupUIArgs.Empty;
        UIMgr.Instance.OpenUIByClick(IncomeExtraPopup.PrefabPath, arg, false, true, null, () => {
            //Debug.Log("on close");
            UpdateTime();
        });
    }
}