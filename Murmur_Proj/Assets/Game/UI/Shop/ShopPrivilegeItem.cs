using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class ShopPrivilegeItem : ShopGiftItemBase
{
    [SerializeField] Text _nameText;
    [SerializeField] Text _titleText;
    [SerializeField] Text _descText;
    [SerializeField] Button _buyBtn;
    [SerializeField] Image _icon;
    [SerializeField] Image _bg;
    [SerializeField] Transform _content;
    [SerializeField] GameObject _boughtRoot;
    [SerializeField] ShopBuyButton _priceButton;

    [SerializeField] ShopGiftRewardItem _incomePrefab;
    [SerializeField] ShopGiftRewardItem _capacityPrefab;

    private ShopGiftPrivilegeConfig _gift;
    // TODO
    private bool isBought = false;

    public override ShopGiftBaseConfig GiftConfig => _gift;

    void Awake()
    {
        _buyBtn.onClick.AddListener(OnClickBuyBtn);
    }

    public void Init(ShopGiftPrivilegeConfig gift)
    {
        _gift = gift;
        isBought = IsAlreadyBought();
        _priceButton.Init(_gift.product_id);

        _icon.ShowSprite(AtlasDefine.GetGiftIconPath(gift.foreground));

        _buyBtn.gameObject.SetActive(!isBought);
        _boughtRoot.gameObject.SetActive(isBought);

        _titleText.text = gift.title.Locale();
        _nameText.text = gift.name.Locale();
        _descText.text = gift.desc.Locale();

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

    private bool IsAlreadyBought()
    {
        return ShopModule.Instance.AlreadyPurchased(_gift.product_id);
    }

    private void SetToBought()
    {
        isBought = IsAlreadyBought();

        _buyBtn.gameObject.SetActive(!isBought);
        _boughtRoot.gameObject.SetActive(isBought);
    }

    private ShopGiftRewardItem GetRewardPrefab(ShopGiftRewardConfig reward) => reward.type switch
    {
        ShopRewardType.Income => _incomePrefab,
        ShopRewardType.MoneyCapacity => _capacityPrefab,
        _ => null,
    };

    public void OnClickBuyBtn()
    {
        AppLogger.Log("buy privlege");
        Buy();
    }

    private void Buy()
    {
        var productId = _gift.product_id;
        ShopModule.Instance.Purchase(productId, (ret) => {
            SetToBought();
        });
    }
}