using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lemegeton;
using UnityEngine.UI;
using System.Threading.Tasks;

public class ShopUICtrl : PopupUIBaseCtrl
{
    public static string PrefabPath = "Assets/Res/UI/Prefab/Shop/ShopUI.prefab";
    public static void Open(string id)
    {
        if (!isOpen) {
            var arg = new ShopUICtrl.Args();
            arg.id = id;
            arg.section = ShopUICtrl.Section.None;
            UIMgr.Instance.OpenUIByClick(PrefabPath, arg, false, true);
        } else {
            using (var arg = ShopUIGotoEventArgs.Get()) {
                arg.id = id;
                arg.section = ShopUICtrl.Section.None;;
                EventModule.Instance.FireEvent(EventDefine.ShopUIGoto, arg);
            }
        }
    }
    public static void Open(Section section = Section.None)
    {
        if (!isOpen) {
            var arg = new ShopUICtrl.Args();
            arg.section = section;
            UIMgr.Instance.OpenUIByClick(PrefabPath, arg, false, true);
        } else {
            using (var arg = ShopUIGotoEventArgs.Get()) {
                arg.id = "";
                arg.section = section;
                EventModule.Instance.FireEvent(EventDefine.ShopUIGoto, arg);
            }
        }

    }

    public enum Section
    {
        None = 0,
        Gift = 1,
        Privilege = 2,
        Builder = 3,
        Researcher = 4,
        Gacha = 5,
        Coin = 6,
    }

    public class Args : PopupUIArgs
    {
        public Section section;
        public string id;
    }

    [SerializeField] RectTransform _content;
    [SerializeField] Transform _titleGift;
    [SerializeField] Transform _titlePrivilege;
    [SerializeField] Transform _titleBuilder;
    [SerializeField] Transform _titleResearcher;
    [SerializeField] Transform _titleGacha;
    [SerializeField] Transform _titleCoin;

    [SerializeField] ShopGiftItem _giftPrefab;
    [SerializeField] ShopPrivilegeItem _privilegefab;

    [SerializeField] ShopBuilderItem _builder;
    [SerializeField] ShopResearcherItem _researcher;


    [SerializeField] ShopGachaItem _gachaBasic;
    [SerializeField] ShopGachaItem _gachaRare;
    [SerializeField] ShopGachaItem _gachaSr;
    [SerializeField] ShopGachaItem _gachaCoupon;

    [SerializeField] Transform _coinContent;
    [SerializeField] ShopCoinItem _shopCoinPrefab;

    private static bool isOpen = false;
    private List<ShopGiftItemBase> giftList = new List<ShopGiftItemBase>();

    public override void Init(PopupUIArgs arg)
    {
        ShopModule.Instance.LoadProducts();
        var param = arg as Args;

        isOpen = true;
        // gift
        var index = _titleGift.transform.GetSiblingIndex();
        var shopConfig = ShopModule.Instance.shopConfig;
        foreach (var gift in ShopModule.Instance.GetValidGiftLimit()) {
            var go = Instantiate(_giftPrefab, _content);
            go.transform.SetSiblingIndex(++index);
            go.Init(gift, true);
            go.shopCtrl = this;
            giftList.Add(go);
        }

        foreach (var gift in shopConfig.shop_gift_normal) {
            var go = Instantiate(_giftPrefab, _content);
            go.transform.SetSiblingIndex(++index);
            go.Init(gift, false);
            go.shopCtrl = this;
            giftList.Add(go);
        }

        index = _titlePrivilege.transform.GetSiblingIndex();
        foreach (var gift in shopConfig.shop_gift_privilege) {
            var go = Instantiate(_privilegefab, _content);
            go.transform.SetSiblingIndex(++index);
            go.Init(gift);
            giftList.Add(go);
        }

        // work team
        _builder.Init();
        _researcher.Init();


        // gacha
        _gachaBasic.Init(shopConfig.shop_gacha["gacha_basic"], true);
        _gachaRare.Init(shopConfig.shop_gacha["gacha_rare"], false);
        _gachaSr.Init(shopConfig.shop_gacha["gacha_sr"], false);
        _gachaCoupon.Init(shopConfig.shop_gacha["gacha_coupon"], false);


        // coin
        foreach (var config in shopConfig.shop_coin) {
            var go = Instantiate(_shopCoinPrefab, _coinContent);
            go.Init(config);
        }

        GotoDelay(param.id, param.section);
    }

    void Awake()
    {
        EventModule.Instance.Register(EventDefine.ShopUIGoto, OnShopUIGotoEvent);
    }

    void OnDestroy()
    {
        EventModule.Instance?.UnRegister(EventDefine.ShopUIGoto, OnShopUIGotoEvent);
        isOpen = false;
    }

    public void OnClickSectionBtn(int id)
    {
        Goto((Section)id);
    }

    private void Goto(Section section)
    {
        var item = section switch
        {
            Section.Gift => _titleGift,
            Section.Privilege => _titlePrivilege,
            Section.Builder => _titleBuilder,
            Section.Researcher => _titleResearcher,
            Section.Gacha => _titleGacha,
            Section.Coin => _titleCoin,
            _ => null,
        };
        if (item == null) {
            return;
        }

        var row = item as RectTransform;
        var pos = -(row.anchoredPosition.y + 0.5f * row.rect.height);

        _content.anchoredPosition = new Vector2(_content.anchoredPosition.x, pos);
    }

    private bool GotoItem(string id)
    {
        foreach (var gift in giftList) {
            if (gift.GiftConfig.id == id) {
                var row = gift.transform as RectTransform;
                var pos = -(row.anchoredPosition.y + 0.5f * row.rect.height);

                _content.anchoredPosition = new Vector2(_content.anchoredPosition.x, pos);

                return true;
            }
        }

        return false;
    }

    private async void GotoDelay(string id, Section section)
    {
        await Task.Delay(100);
        if (GotoItem(id)) {
            return;
        }

        if (section == Section.None) {
            return;
        }

        Goto(section);
    }

    public void ResetTeam()
    {
        _builder.Init();
        _researcher.Init();
    }
    private void OnShopUIGotoEvent(Component sender, System.EventArgs e)
    {
        var param = e as ShopUIGotoEventArgs;

        if (GotoItem(param.id)) {
            return;
        }

        Goto(param.section);

        var last = this.transform.parent.childCount - 1;
        if (this.transform.GetSiblingIndex() < last) {
            // ahead
            this.transform.SetSiblingIndex(last);
        }
    }
    protected override void OnBeforeHide()
    {
        isOpen = false;
    }
}
