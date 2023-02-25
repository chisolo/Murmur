using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lemegeton;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;

public class ServiceBoostPopup : PopupUIBaseCtrl
{
    public static string PrefabPath = "Assets/Res/UI/Prefab/MainPopup/ServiceBoostPopup.prefab";

    public static void Open()
    {
        var arg = PopupUIArgs.Empty;
        UIMgr.Instance.OpenUIByClick(PrefabPath, arg, false, true);
    }

    [SerializeField] Text _effectText;
    [SerializeField] GameObject _giftRoot;

    private ServiceBoostConfig config => ConfigModule.Instance.ServiceBoost();

    public override void Init(PopupUIArgs arg)
    {
        var build = BuildingModule.Instance.IsBuildingReady(config.require_build_id);
        var isVip = ShopModule.Instance.IsVip;

        _giftRoot.SetActive(build && !isVip);

        _effectText.text = string.Format("SERVICE_BOOST_EFFECT_TIME".Locale(), config.effect_time);
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

    private int cooltime = 3;
    private bool closeByAd = false;

    private void WatchAd()
    {
        AdModule.Instance.ShowRewardAd(AdUnitName.ad_unit_reward_service_boost, () => {
            ///Debug.Log("bost end");
            BuffModule.Instance.AddServiceBoostBuff();
            closeByAd = true;

            this.Hide();
        });
    }

    protected override void OnBeforeHide()
    {
        //AdModule.Instance.AddExtraCashTime(cooltime);
        AdModule.Instance.AddServiceBoostValidTime(10);
    }

    protected override void OnBeforeClose()
    {
        if (!closeByAd) {
            PromotionPopup.OpenByPopup();
        }
    }

}