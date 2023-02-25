using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class ServiceBoostButton : MonoBehaviour
{

    enum Status
    {
        LOCK,
        NORMAL,
        EFFECT,
        HIDE,
    }
    [SerializeField]
    private Button _button;
    [SerializeField] Transition _trans;

    [SerializeField] Text _timeText;
    [SerializeField] GameObject _normal;
    [SerializeField] GameObject _inEffect;

    private long _timerId = -1;
    private Status _status;

    private ServiceBoostConfig config => ConfigModule.Instance.ServiceBoost();

    void Awake()
    {
        _button.onClick.AddListener(OnClickButton);

        var now = NtpModule.Instance.UtcNowSeconds;
        var end = BuffModule.Instance.serviceBoostEndTime;

        if (PlayerModule.Instance.Star < config.unlock_star_num) {
            _trans.Right();
            _normal.SetActive(true);
            _inEffect.SetActive(false);
            _status = Status.LOCK;
        } else if (now < end) {
            _timeText.text = FormatUtil.FormatTimeShort(end - now);
            _normal.SetActive(false);
            _inEffect.SetActive(true);
            _status = Status.EFFECT;
        } else {
            _trans.Right();
            _normal.SetActive(true);
            _inEffect.SetActive(false);
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
        var end = BuffModule.Instance.serviceBoostEndTime;
        var validTime = AdModule.Instance.ServiceBoostValidTime;

        if (_status == Status.LOCK && PlayerModule.Instance.Star >= config.unlock_star_num) {
            _status = Status.NORMAL;
            _normal.SetActive(true);
            _inEffect.SetActive(false);
            _trans.Back();
            return;
        }

        if (_status == Status.NORMAL && now < end) {
            _timeText.text = FormatUtil.FormatTimeShort(end - now);
            _normal.SetActive(false);
            _inEffect.SetActive(true);
            _status = Status.EFFECT;
            return;
        }

        if (_status == Status.EFFECT && now < end) {
            _timeText.text = FormatUtil.FormatTimeShort(end - now);
            return;
        }

        if (_status == Status.EFFECT && now >= end) {
            _trans.Right();
            _status = Status.HIDE;
            return;
        }

        if (_status == Status.NORMAL && now < validTime) {
            _trans.Right();
            _status = Status.HIDE;
            return;
        }

        if (_status == Status.HIDE && AdModule.Instance.HasRewarded && now >= validTime) {
            _status = Status.NORMAL;
            _normal.SetActive(true);
            _inEffect.SetActive(false);
            _trans.Back();
            return;
        }
    }

    public void OnClickButton()
    {
        if (_status != Status.NORMAL) {
            return;
        }
        //ServiceBoostPopup.Open();

        var arg = PopupUIArgs.Empty;
        UIMgr.Instance.OpenUIByClick(ServiceBoostPopup.PrefabPath, arg, false, true, null, () => {
            //Debug.Log("on close");
            UpdateTime();
        });
    }
}