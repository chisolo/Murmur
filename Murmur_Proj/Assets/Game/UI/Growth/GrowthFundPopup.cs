using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lemegeton;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;
using System;

public class GrowthFundPopup : PopupUIBaseCtrl
{
    public static string PrefabPath = "Assets/Res/UI/Prefab/Growth/GrowthFundPopup.prefab";

    public class Args : PopupUIArgs
    {
        public ShopGiftConfig gift;
    }

    [SerializeField] Text _lvText;
    [SerializeField] Text _getText;
    [SerializeField] Button _buyBtn;
    [SerializeField] ShopBuyButton _priceButton;
    [SerializeField] GameObject _boughtRoot;

    [SerializeField] Transform _content;
    [SerializeField] GrowthFundItem _itemPrefab;


    public static void Open()
    {
        UIMgr.Instance.OpenUIByClick(PrefabPath, PopupUIArgs.Empty, false, true);
    }

    public override void Init(PopupUIArgs arg)
    {
        var param = arg as Args;

        var config = ConfigModule.Instance.GrowthFund();

        _buyBtn.onClick.RemoveAllListeners();
        _buyBtn.onClick.AddListener(OnClickBuyBtn);

        _priceButton.Init(config.product_id);

        _lvText.text = PlayerModule.Instance.Star.ToString();
        _getText.text = AllRewardCoin().ToString();

        SetupRewards();

        ShopModule.Instance.LoadProducts();
    }

    int AllRewardCoin()
    {
        return GrowthFundModule.Instance.AllRewardCoin();
    }

    void SetupRewards()
    {
        AppLogger.Log("SetupRewards");
        var config = ConfigModule.Instance.GrowthFund();
        var own = ShopModule.Instance.AlreadyPurchased(config.product_id);

        _priceButton.gameObject.SetActive(!own);
        _boughtRoot.SetActive(own);

        foreach (Transform child in _content) {
            Destroy(child.gameObject);
        }

        foreach (var reward in config.rewards) {
            var item = Instantiate(_itemPrefab, _content);
            item.Init(reward);
        }
    }

    public void OnClickBuyBtn()
    {
        AppLogger.Log("buy gift");
        Buy();
    }

    private void Buy()
    {
        var config = ConfigModule.Instance.GrowthFund();
        var productId = config.product_id;
        ShopModule.Instance.Purchase(productId, (ret) => {
            SetupRewards();
        });
    }

}