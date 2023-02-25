using System;
using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class ShopBuyButton : MonoBehaviour
{
    [SerializeField] Text _priceText;
    [SerializeField] Button _buyBtn;
    [SerializeField] GameObject _loading;

    private int _timerId;
    private string _productId;

    private bool _hasPrice = false;

    public void Init(string productId)
    {
        _productId = productId;
        _hasPrice = false;

        var price = ShopModule.Instance.GetPrice(_productId);
        if (!string.IsNullOrEmpty(price)) {
            _priceText.text = price;
            _loading.SetActive(false);
            _hasPrice = true;
        } else {
            _loading.SetActive(true);
            _priceText.gameObject.SetActive(false);
            _buyBtn.interactable = false;
        }

    }

    void Awake()
    {
        EventModule.Instance.Register(EventDefine.ProductListLoaded, OnProductListLoaded);
    }

    void OnDestroy()
    {
        EventModule.Instance?.UnRegister(EventDefine.ProductListLoaded, OnProductListLoaded);
    }

    void Update()
    {
        if (_hasPrice) return;
        TryGetPrice();
    }

    private void TryGetPrice()
    {
        if (!_priceText.gameObject.activeSelf) {
            var price = ShopModule.Instance.GetPrice(_productId);
            if (!string.IsNullOrEmpty(price)) {
                _priceText.text = price;
                _priceText.gameObject.SetActive(true);
                _buyBtn.interactable = true;
                _loading.SetActive(false);

                _hasPrice = true;
            } else {
                //Debug.LogError("no product " + _productId);
            }
        }
    }

    private void OnProductListLoaded(Component sender, System.EventArgs e)
    {
        TryGetPrice();
    }

}