using System;
using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class ShopGiftItem : ShopGiftItemBase
{
    [SerializeField] Text _discountText;
    [SerializeField] Text _titleText;
    [SerializeField] Button _buyBtn;
    [SerializeField] Transform _content;
    [SerializeField] Image _bg;
    [SerializeField] Text _timeText;
    [SerializeField] GameObject _timeRoot;
    [SerializeField] GameObject _boughtRoot;
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
    public ShopUICtrl shopCtrl;

    private bool isBought = false;

    void Awake()
    {
        _buyBtn.onClick.AddListener(OnClickBuyBtn);
    }

    public override ShopGiftBaseConfig GiftConfig => _gift;

    public void Init(ShopGiftConfig gift, bool limit)
    {
        _gift = gift;
        isBought = IsAlreadyBought();
        _priceButton.Init(_gift.product_id);

        _discountText.text = $"-{gift.discount}%";
        _titleText.text = gift.title.Locale();

        _buyBtn.gameObject.SetActive(!isBought);
        _boughtRoot.gameObject.SetActive(isBought);

        _bg.ShowSprite(AtlasDefine.GetGiftPath(gift.background));

        if (limit && !isBought) {
            _timeRoot.SetActive(true);
            _time = ShopModule.Instance.GetGiftVaildTime(gift.id);
            var remain = _time - NtpModule.Instance.UtcNowSeconds;
            UpateTime();

            _timerId = TimerModule.Instance.CreateTimer(remain, () => {
                // TOOD: end
                Destroy(this.gameObject);
            }, true, (x) => {
                UpateTime();
            });
        } else {
            _timeRoot.SetActive(false);
        }

        foreach (var reward in ShopModule.Instance.GetProductRewards(gift.product_id)) {
            var prefab = GetRewardPrefab(reward);
            if (prefab != null) {
                var go = Instantiate(prefab, _content);
                go.Init(reward.value, ShopModule.Instance.GetRewardName(reward.type));
            } else {
                Debug.LogError($"no prefab {reward.type} : {reward.style}");
            }
        }
    }

    private void SetToBought()
    {
        isBought = IsAlreadyBought();

        _buyBtn.gameObject.SetActive(!isBought);
        _boughtRoot.gameObject.SetActive(isBought);
        _timeRoot.SetActive(!isBought);
    }

    private bool IsAlreadyBought()
    {
        return ShopModule.Instance.AlreadyPurchased(_gift.product_id);
    }

    void OnDestroy()
    {
        TimerModule.Instance?.CancelTimer(_timerId);
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

    public void OnClickBuyBtn()
    {
        AppLogger.Log("buy gift");
        Buy();
    }

    private void Buy()
    {
        var productId = _gift.product_id;
        ShopModule.Instance.Purchase(productId, (ret) => {
            shopCtrl.ResetTeam();
            SetToBought();
        });
    }

    private void UpateTime()
    {
        //var time = ShopModule.Instance.GetGiftVaildTime(_gift.id);
        var remain = TimeSpan.FromSeconds(_time - NtpModule.Instance.UtcNowSeconds);
        _timeText.text = remain.ToString(@"h\:mm\:ss");
    }


}