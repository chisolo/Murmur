using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lemegeton;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;

public class MoreGuestPopup : PopupUIBaseCtrl
{

    public static string PrefabPath = "Assets/Res/UI/Prefab/MainPopup/MoreGuestPopup.prefab";

    public static void Open()
    {
        var arg = PopupUIArgs.Empty;
        UIMgr.Instance.OpenUIByClick(PrefabPath, arg, false, true);
    }

    [SerializeField] Text _guestText;
    [SerializeField] GameObject _giftRoot;

    private MoreGuestConfig config => ConfigModule.Instance.MoreGuest();

    public override void Init(PopupUIArgs arg)
    {
        var build = BuildingModule.Instance.IsBuildingReady(config.require_build_id);
        var isVip = ShopModule.Instance.IsVip;

        _giftRoot.SetActive(build && !isVip);

        _guestText.text = string.Format("MORE_GUEST_NEW_COUNT_DESC".Locale(), config.new_guest_count);
    }

    public void OnClickShopBtn()
    {
        //this.Hide();
        AppLogger.Log("OnClickShopBtn");

        var limit = config.shop_limit_gift_id;
        var gift = ShopModule.Instance.GetVaildGiftConfig(limit);
        if (gift != null) {
            PromotionPopup.Open(gift);
            //this.Hide();
        } else {
            var id = config.shop_gift_id;
            ShopUICtrl.Open(id);
        }
    }

    public void OnClickAdBtn()
    {
        WatchAd();
    }

    private void WatchAd()
    {
        AdModule.Instance.ShowRewardAd(AdUnitName.ad_unit_reward_more_guest, () => {
            Debug.Log("WatchAd end");
            PuppetMgr.Instance.InstantSpawnGuest(config.new_guest_count);
            AdModule.Instance.AddMoreGuestValidTime(config.cooldown);

            closeByAd = true;
            this.Hide();
        });
    }

    public void SelfClose()
    {
        AdModule.Instance.AddMoreGuestValidTime(config.cooldown);
        this.Hide();
    }

    private bool closeByAd = false;
    protected override void OnBeforeClose()
    {
        if (!closeByAd) {
            PromotionPopup.OpenByPopup();
        }
    }


}