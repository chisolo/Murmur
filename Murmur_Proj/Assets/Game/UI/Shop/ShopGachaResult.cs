using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;

public class ShopGachaResult : PopupUIBaseCtrl
{

    public static string PrefabPath = "Assets/Res/UI/Prefab/Shop/ShopGachaResult.prefab";

    public class Args : PopupUIArgs
    {
        public string staffId;
    }

    public static void Open(string id)
    {
        var arg = new Args();
        arg.staffId = id;

        UIMgr.Instance.OpenUI(PrefabPath, arg, true, false);
    }

    [SerializeField]
    private StaffItem _staffItem;
    [SerializeField] Text CouponText;
    [SerializeField] Text GetCouponText;

    private string _staffId;

    public override void Init(PopupUIArgs arg)
    {
        var param = (Args)arg;
        _staffId = param.staffId;
        var staffData = StaffModule.Instance.GetArchiveData(param.staffId);

        _staffItem.Init(staffData);

        var get = 0;
        if (staffData.rarity == StaffRarity.RARE) {
            get = StaffDefine.CouponRare;
        } else if (staffData.rarity == StaffRarity.SR) {
            get = StaffDefine.CouponSR;
        }

        GetCouponText.text = get.ToString();
        if (get == 0) {
            GetCouponText.gameObject.SetActive(false);
        }

        CouponText.text = FormatUtil.Currency(PlayerModule.Instance.Coupon, false);
    }

    public void OnClickFire()
    {
        var staffData = StaffModule.Instance.GetArchiveData(_staffId);
        var get = 0;
        if (staffData.rarity == StaffRarity.RARE) {
            get = StaffDefine.CouponRare;
        } else if (staffData.rarity == StaffRarity.SR) {
            get = StaffDefine.CouponSR;
        }

        StaffModule.Instance.FireStaff(_staffId);
        if (get > 0) {
            PlayerModule.Instance.AddCoupon(get);
            CouponText.text = FormatUtil.Currency(PlayerModule.Instance.Coupon, false);
        }

        this.Hide();
    }

}