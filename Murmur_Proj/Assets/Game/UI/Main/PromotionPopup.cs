using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lemegeton;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;
using System;

public class PromotionPopup : PopupUIBaseCtrl
{
    public static string PrefabPath = "Assets/Res/UI/Prefab/MainPopup/PromotionPopup.prefab";

    public class Args : PopupUIArgs
    {
        public ShopGiftConfig gift;
    }

    [SerializeField] Text _discountText;
    [SerializeField] Text _titleText;
    [SerializeField] Button _buyBtn;
    [SerializeField] Transform _content;
    [SerializeField] Image _bg;
    [SerializeField] Text _timeText;
    [SerializeField] ShopBuyButton _priceButton;

    [SerializeField] ShopGiftRewardItem _incomePrefab;
    [SerializeField] ShopGiftRewardItem _capacityBigPrefab;
    [SerializeField] ShopGiftRewardItem _capacitySmallPrefab;
    [SerializeField] ShopGiftRewardItem _coinPrefab;
    [SerializeField] ShopGiftRewardItem _builderPrefab;
    [SerializeField] ShopGiftRewardItem _researcherPrefab;
    [SerializeField] ShopGiftRewardItem _noadsPrefab;

    private long _timerId;
    private long _time;
    private ShopGiftConfig _gift;

    public static void OpenByPopup()
    {
        var config = ConfigModule.Instance.Common();
        if (PlayerModule.Instance.Star < config.promotion_popup_show_star_count) {
            return;
        }
        if (!AdModule.Instance.canShowPromotion) {
            return;
        }
        if (AdModule.Instance.hasShowPromotion) {
            return;
        }
        var gift = ShopModule.Instance.PromotionGift;
        if (gift == null) {
            return;
        }

        AdModule.Instance.hasShowPromotion = true;
        Open(gift);
    }

    public static void Open()
    {
        if (PlayerModule.Instance.Star < ConfigModule.Instance.Common().promotion_popup_show_star_count) {
            return;
        }

        var gift = ShopModule.Instance.PromotionGift;
        Open(gift);
    }

    public static void Open(ShopGiftConfig gift)
    {
        if (gift == null) {
            return;
        }

        var time = ShopModule.Instance.GetGiftVaildTime(gift.id);
        var remain = time - NtpModule.Instance.UtcNowSeconds;
        if (remain <= 0) {
            return;
        }

        var arg = new Args();
        arg.gift = gift;
        UIMgr.Instance.OpenUI(PrefabPath, arg, false, true);
    }

    public override void Init(PopupUIArgs arg)
    {
        var param = arg as Args;
        var gift = param.gift;
        _gift = gift;

        _buyBtn.onClick.RemoveAllListeners();
        _buyBtn.onClick.AddListener(OnClickBuyBtn);

        _priceButton.Init(_gift.product_id);

        _discountText.text = $"-{gift.discount}%";
        _titleText.text = gift.title.Locale();

        _bg.ShowSprite(AtlasDefine.GetMainPath(gift.popup_icon));

        //_timeRoot.SetActive(true);
        _time = ShopModule.Instance.GetGiftVaildTime(gift.id);
        var remain = _time - NtpModule.Instance.UtcNowSeconds;
        UpateTime();

        _timerId = TimerModule.Instance.CreateTimer(remain, () => {
            // TOOD: end
            this.Hide();
        }, true, (x) => {
            UpateTime();
        });

        foreach (var reward in ShopModule.Instance.GetProductRewards(gift.product_id)) {
            var prefab = GetRewardPrefab(reward);
            if (prefab != null) {
                var go = Instantiate(prefab, _content);
                go.Init(reward.value, ShopModule.Instance.GetRewardName(reward.type));
            } else {
                Debug.LogError($"no prefab {reward.type} : {reward.style}");
            }
        }

        ShopModule.Instance.LoadProducts();
        TaskModule.Instance.Delay(3000, () => {
            var price = ShopModule.Instance?.GetPrice(gift.product_id);
            if (string.IsNullOrEmpty(price)) {
                ShopModule.Instance?.LoadProducts();
            }
        });
    }

    void OnDestroy()
    {
        TimerModule.Instance?.CancelTimer(_timerId);
    }

    public void OnClickBuyBtn()
    {
        AppLogger.Log("buy gift");
        Buy();
    }

    private void Buy()
    {
        var productId = _gift.product_id;
        ShopModule.Instance.Purchase(productId, (ret) => {
            this.Hide();
        });
    }

    private ShopGiftRewardItem GetRewardPrefab(ShopGiftRewardConfig reward) => (reward.type, reward.style) switch
    {
        (ShopRewardType.Income, _) => _incomePrefab,
        (ShopRewardType.MoneyCapacity, "big") => _capacityBigPrefab,
        (ShopRewardType.MoneyCapacity, "small") => _capacitySmallPrefab,
        (ShopRewardType.Coin, _) => _coinPrefab,
        (ShopRewardType.BuilderTeam, _) => _builderPrefab,
        (ShopRewardType.ResercherTeam, _) => _researcherPrefab,
        (ShopRewardType.NoAds, _) => _noadsPrefab,
        _ => null,
    };

    private void UpateTime()
    {
        //var time = ShopModule.Instance.GetGiftVaildTime(_gift.id);
        var remain = TimeSpan.FromSeconds(_time - NtpModule.Instance.UtcNowSeconds);
        _timeText.text = remain.ToString(@"h\:mm\:ss");
    }


}