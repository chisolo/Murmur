using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lemegeton;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;
public class IncomeExtraPopup : PopupUIBaseCtrl
{
    public static string PrefabPath = "Assets/Res/UI/Prefab/MainPopup/IncomeExtraPopup.prefab";

    public static void Open()
    {
        var arg = PopupUIArgs.Empty;
        UIMgr.Instance.OpenUIByClick(PrefabPath, arg, false, true);
    }

    [SerializeField] Text _getMoneyText;
    [SerializeField] GameObject _giftRoot;

    private CommonConfig config => ConfigModule.Instance.Common();

    private int _get;

    public override void Init(PopupUIArgs arg)
    {
        var build = BuildingModule.Instance.IsBuildingReady(config.ad_extra_require_build_id);
        var isVip = ShopModule.Instance.IsVip;

        _giftRoot.SetActive(build && !isVip);
        var factor = 1f;
        if (config.ad_extra_factor != null) {
            factor = RandUtil.RandomOne(config.ad_extra_factor);
        }

        var num = (int)(BuildingModule.Instance.GetTotalProfit() * factor);
        var str = num.ToString();
        var sb = new System.Text.StringBuilder(str);
        for (int i = 2; i < str.Length; i++) {
            sb[i] = '0';
        }

        _get = int.Parse(sb.ToString());
        _getMoneyText.text = FormatUtil.Revenue(_get, false);

    }

    public void OnClickShopBtn()
    {
        //this.Hide();
        AppLogger.Log("OnClickShopBtn");

        var limit = config.ad_extra_shop_limit_gift_id;
        var gift = ShopModule.Instance.GetVaildGiftConfig(limit);
        if (gift != null) {
            PromotionPopup.Open(gift);
            //this.Hide();
        } else {
            var id = config.ad_extra_shop_gift_id;
            ShopUICtrl.Open(id);
        }
    }

    public void OnClickAdBtn()
    {
        WatchAd();
    }

    private int cooltime = 10;
    private bool closeByAd = false;

    private void WatchAd()
    {
        AdModule.Instance.ShowRewardAd(AdUnitName.ad_unit_reward_extra_cash, () => {
            PlayerModule.Instance.AddMoneyWithBigEffect(_get, true);
            cooltime = config.ad_extra_button_cooldown;
            closeByAd = true;
            this.Hide();
        });
    }

    protected override void OnBeforeHide()
    {
        AdModule.Instance.AddExtraCashTime(cooltime);
    }

    protected override void OnBeforeClose()
    {
        if (!closeByAd) {
            PromotionPopup.OpenByPopup();
        }
    }

}