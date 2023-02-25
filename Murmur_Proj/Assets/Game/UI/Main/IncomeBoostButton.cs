using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class IncomeBoostButton : MonoBehaviour
{
    [SerializeField]
    private Button _button;
    [SerializeField]
    private Text _remainTime;

    [SerializeField]
    private GameObject _inboost;
    [SerializeField]
    private GameObject _normal;


    [SerializeField] UpDown _upDown;

    private long _timerId = -1;
    private bool active = false;

    void Awake()
    {
        _button.onClick.AddListener(OnClickButton);
        UpdateTime();
        _timerId = TimerModule.Instance.CreateTimer(1, () => {
            UpdateTime();
        }, loop: -1);

        var now = NtpModule.Instance.UtcNowSeconds;
        if (BuffModule.Instance.adIncomeBoostEndTime > now) {
            active = true;
            _inboost.gameObject.SetActiveIfNeed(true);
            _normal.gameObject.SetActiveIfNeed(false);
        } else {
            active = false;
            _inboost.gameObject.SetActiveIfNeed(false);
            _normal.gameObject.SetActiveIfNeed(true);
        }


        _upDown.Play();
        if (AnimeShow()) {
            _upDown.gameObject.SetActive(true);
            _animeOn = true;
        } else {
            _upDown.gameObject.SetActive(false);
            _animeOn = false;
        }
    }

    void OnDestroy()
    {
        TimerModule.Instance?.CancelTimer(_timerId);
    }

    void UpdateTime()
    {
        var now = NtpModule.Instance.UtcNowSeconds;
        if (BuffModule.Instance.adIncomeBoostEndTime > now && !active) {
            active = true;
            _inboost.gameObject.SetActiveIfNeed(true);
            _normal.gameObject.SetActiveIfNeed(false);
        } else if (BuffModule.Instance.adIncomeBoostEndTime <= now && active) {
            active = false;
            _inboost.gameObject.SetActiveIfNeed(false);
            _normal.gameObject.SetActiveIfNeed(true);
        }


        if (BuffModule.Instance.adIncomeBoostEndTime > now) {
            var remain = BuffModule.Instance.adIncomeBoostEndTime - now;
            _remainTime.text = FormatUtil.FormatTimeAuto(remain);
        }


        _animDurtion += 1;
        if (AnimeShow()) {
            if (_animeOn && _animDurtion >= 6) {
                _upDown.gameObject.SetActiveIfNeed(false);
                _animDurtion = 0;
                _animeOn = false;
            } else if (!_animeOn && _animDurtion >= 4) {
                _upDown.gameObject.SetActiveIfNeed(true);
                _animDurtion = 0;
                _animeOn = true;
            }
        } else {
            _upDown.gameObject.SetActiveIfNeed(false);
        }
    }

    float _animDurtion;
    bool _animeOn;

    public void OnClickButton()
    {
        IncomeBoostPopup.Open();
    }

    bool AnimeShow()
    {
        var now = NtpModule.Instance.UtcNowSeconds;
        var config = ConfigModule.Instance.Common();
        return PlayerModule.Instance.Star >= config.income_boost_anim_unlock_star
            && BuffModule.Instance.adIncomeBoostEndTime <= now;

    }
}