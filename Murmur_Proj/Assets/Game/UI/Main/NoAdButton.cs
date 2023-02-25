using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class NoAdButton : MonoBehaviour
{
    [SerializeField] Button _button;
    [SerializeField] Transition _trans;

    private long _timerId = -1;

    bool isHide = false;

    private CommonConfig config => ConfigModule.Instance.Common();
    void Awake()
    {
        //Debug.Log("config.no_ad_show_star_count " + config.no_ad_show_star_count);
        _button.onClick.AddListener(OnClickButton);

        var isVip = ShopModule.Instance.IsVip;

        if (isVip) {
            // hide
            _trans.Left();
        } else {
            if (PlayerModule.Instance.Star < config.no_ad_show_star_count) {
                _trans.Left();
                isHide = true;
            }
            _timerId = TimerModule.Instance.CreateTimer(1f, () => {
                UpdateTime();
            }, loop: -1);
        }
    }

    void OnDestroy()
    {
        TimerModule.Instance?.CancelTimer(_timerId);
    }

    void UpdateTime()
    {
        var isVip = ShopModule.Instance.IsVip;
        if (isVip) {
            // hide
            _trans.Left();
            TimerModule.Instance?.CancelTimer(_timerId);
        }

        if (isHide && !isVip && PlayerModule.Instance.Star >= config.no_ad_show_star_count) {
            _trans.Back();
            isHide = false;
        }
    }

    public void OnClickButton()
    {
        var id = config.ad_extra_shop_gift_id;
        ShopUICtrl.Open(id);
    }
}