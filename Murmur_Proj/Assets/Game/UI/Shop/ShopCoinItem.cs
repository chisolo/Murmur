using System;
using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class ShopCoinItem : MonoBehaviour
{
    [SerializeField] Button _buyBtn;
    [SerializeField] Text _amountText;
    [SerializeField] GameObject _popular;
    [SerializeField] Image _icon;
    [SerializeField] ShopBuyButton _priceButton;

    private ShopCoinConfig _config;
    private bool _isBasic;

    void Awake()
    {
        _buyBtn.onClick.AddListener(OnClickBuyBtn);
    }

    public void Init(ShopCoinConfig config)
    {
        _config = config;

        _popular.SetActive(config.popular);
        _amountText.text = FormatUtil.Currency(config.num);

        _priceButton.Init(_config.product_id);

        _icon.ShowSprite(AtlasDefine.GetGiftIconPath(config.foreground));
    }

    public void OnClickBuyBtn()
    {
        AppLogger.Log("buy coin");
        Buy();
    }

    private void Buy()
    {
        var productId = _config.product_id;
        ShopModule.Instance.Purchase(productId, (ret) => {

        });
    }
}