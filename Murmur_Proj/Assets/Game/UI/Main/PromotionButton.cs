using System;
using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class PromotionButton : MonoBehaviour
{
    [SerializeField] Button _button;
    [SerializeField] Transition _trans;
    [SerializeField] Image _icon;

    [SerializeField] Text _titleText;
    [SerializeField] Text _timeText;

    private long _timerId = -1;

    private ShopGiftConfig _config;
    void Start()
    {
        _config = ShopModule.Instance.PromotionGift;
        _button.onClick.AddListener(OnClickButton);

        if (_config == null) {
            _trans.Left();
            return;
        }

        var now = NtpModule.Instance.UtcNowSeconds;
        var time = ShopModule.Instance.GetGiftVaildTime(_config.id);
        if (time < now) {
            _trans.Left();
            return;
        }

        _titleText.text = _config.title.Locale();
        var remain = TimeSpan.FromSeconds(time - now);
        _timeText.text = remain.ToString(@"h\:mm\:ss");

        _icon.ShowSprite(AtlasDefine.GetMainPath(_config.main_ui_icon));

        _timerId = TimerModule.Instance.CreateTimer(1f, () => {
            UpdateTime();
        }, loop: -1);

    }

    void OnDestroy()
    {
        TimerModule.Instance?.CancelTimer(_timerId);
    }

    void UpdateTime()
    {
        if (_config == null) {
            return;
        }

        var now = NtpModule.Instance.UtcNowSeconds;
        var time = ShopModule.Instance.GetGiftVaildTime(_config.id);
        if (time < now) {
            _trans.Left();
            TimerModule.Instance?.CancelTimer(_timerId);
            return;
        }

        var remain = TimeSpan.FromSeconds(time - now);
        _timeText.text = remain.ToString(@"h\:mm\:ss");

        var own = ShopModule.Instance.AlreadyPurchased(_config.product_id);
        if (own) {
            _trans.Left();
            TimerModule.Instance?.CancelTimer(_timerId);
            return;
        }
    }

    public void OnClickButton()
    {
        //AppLogger.Log("main OnClickButton ");
        PromotionPopup.Open(_config);
    }
}