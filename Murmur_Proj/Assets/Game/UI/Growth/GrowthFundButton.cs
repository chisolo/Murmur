using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class GrowthFundButton : MonoBehaviour
{
    [SerializeField] Button _button;
    [SerializeField] Transition _trans;
    [SerializeField] GameObject _notReceivedMark;

    private long _timerId = -1;

    private GrowthFundConfig config => ConfigModule.Instance.GrowthFund();
    void Awake()
    {
        //Debug.Log("config.no_ad_show_star_count " + config.no_ad_show_star_count);
        _button.onClick.AddListener(OnClickButton);

        if (PlayerModule.Instance.Star < config.unlock_star_num) {
            _trans.Left();

            _timerId = TimerModule.Instance.CreateTimer(1f, () => {
                UpdateTime();
            }, loop: -1);
        }

        CheckNotReceived();
        EventModule.Instance.Register(EventDefine.UpdateItem, OnUpdateItemEvent);
    }

    void OnDestroy()
    {
        TimerModule.Instance?.CancelTimer(_timerId);
        EventModule.Instance?.UnRegister(EventDefine.UpdateItem, OnUpdateItemEvent);
    }

    void UpdateTime()
    {
        if (PlayerModule.Instance.Star >= config.unlock_star_num) {
            _trans.Back();

            TimerModule.Instance?.CancelTimer(_timerId);
        }
    }

    public void OnClickButton()
    {
        //var id = config.ad_extra_shop_gift_id;
        //ShopUICtrl.Open(id);
        //GrowthFundPopup.Open();
        UIMgr.Instance.OpenUIByClick(GrowthFundPopup.PrefabPath, PopupUIArgs.Empty, false, true, null, () => {
            Debug.Log("on close");
            CheckNotReceived();
        });
    }

    void CheckNotReceived()
    {
        _notReceivedMark.SetActiveIfNeed(GrowthFundModule.Instance.GetNotReceivedCount() > 0);
    }

    private void OnUpdateItemEvent(Component sender, System.EventArgs e)
    {
        var args = e as UpdateItemArgs;
        if (args.item == ItemType.Star) {
            CheckNotReceived();
        }
    }
}