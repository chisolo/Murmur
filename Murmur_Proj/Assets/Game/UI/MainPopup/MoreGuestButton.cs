using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class MoreGuestButton : MonoBehaviour
{

    enum Status
    {
        LOCK,
        NORMAL,
        HIDE,
    }
    [SerializeField]
    private Button _button;
    [SerializeField] Transition _trans;


    private long _timerId = -1;
    private Status _status;

    private MoreGuestConfig config => ConfigModule.Instance.MoreGuest();

    void Awake()
    {
        _button.onClick.AddListener(OnClickButton);

        if (PlayerModule.Instance.Star < config.unlock_star_num) {
            _trans.Right();
            _status = Status.LOCK;
        } else {
            _trans.Right();
            _status = Status.HIDE;
        }

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
        var validTime = AdModule.Instance.MoreGuestValidTime;

        if (_status == Status.LOCK && PlayerModule.Instance.Star >= config.unlock_star_num) {
            _status = Status.NORMAL;
            _trans.Back();
            return;
        }

        if (_status == Status.NORMAL && now < validTime) {
            _trans.Right();
            _status = Status.HIDE;
            return;
        }

        if (_status == Status.HIDE && AdModule.Instance.HasRewarded && now >= validTime) {
            _status = Status.NORMAL;
            _trans.Back();
            return;
        }
    }

    public void OnClickButton()
    {
        //MoreGuestPopup.Open();
        var arg = PopupUIArgs.Empty;
        UIMgr.Instance.OpenUIByClick(MoreGuestPopup.PrefabPath, arg, false, true, null, () => {
            //Debug.Log("on close");
            UpdateTime();
        });
    }
}