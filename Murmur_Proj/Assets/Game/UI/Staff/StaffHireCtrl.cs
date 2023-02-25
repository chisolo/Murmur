using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lemegeton;
using UnityEngine.UI;

public class StaffHireCtrl : PopupUIBaseCtrl
{
    public class StaffHireCtrlArgs : PopupUIArgs
    {
        public string buildingId;
    }

    public static string PrefabPath = "Assets/Res/UI/Prefab/Popup/StaffHireUI.prefab";

    [SerializeField]
    private StaffHireView _hireView;
    [SerializeField]
    private StaffListView _listView;
    [SerializeField]
    private Text[] _staffCountTabTextList;

    private CandidateData _candidateData;
    private string _buildingId;

    public override void Init(PopupUIArgs arg)
    {
        var param = (StaffHireCtrlArgs)arg;
        _buildingId = param.buildingId;

        if (StaffModule.Instance.IsCandidateExpire()) {
            StaffModule.Instance.RefreshCandidates(false);
        }

        if (StaffModule.Instance.IsRefreshCountResetTimeExpire()) {
            StaffModule.Instance.ResetRefreshCount();
        }

        _candidateData = StaffModule.Instance.GetCandidateData();
        _hireView.Init(_candidateData);

        var staffData = StaffModule.Instance.GetStaffData();
        _listView.Init(staffData);
        _listView.onClickStaffAction = OnClickStaff;

        SetStaffCount();
    }

    private void Update()
    {
        if (StaffModule.Instance.IsCandidateExpire()) {
            Refresh(false);
        }
    }

    private void Refresh(bool byAd)
    {
        StaffModule.Instance.RefreshCandidates(byAd);
        _hireView.RefreshView();
    }

    public void OnClickMakeOffer()
    {
        if (StaffModule.Instance.GetStaffCount() >= StaffModule.Instance.GetStaffMaxCount()) {
            UIMgr.Instance.OpenConfirmOk(StaffText.StaffFullTitleText.Locale(), StaffText.StaffFullBodyText.Locale(), StaffText.Accept.Locale());
            return;
        }
        //AppLogger.Log("OnClickMakeOffer");

        var hiredStaffId = _hireView.MakeOffer();
        SetStaffCount();

        //AppLogger.LogFormat("offer result staff {0}, build {1}", hiredStaffId, _buildingId);

        if (!string.IsNullOrEmpty(_buildingId) && !string.IsNullOrEmpty(hiredStaffId)) {
            StaffModule.Instance.AssignBuilding(hiredStaffId, _buildingId);
        }
    }

    public void OnClickRefresh()
    {
        if (!StaffModule.Instance.HasRefreshCount()) {
            return;
        }

        WatchAd();
    }

    private void WatchAd()
    {
        AdModule.Instance.ShowRewardAd(AdUnitName.ad_unit_reward_refresh_staff, () => {
            Refresh(true);
        });
    }

    public void OnClickOfferDown()
    {
        //AppLogger.Log("OnClickOfferDown");
        _hireView.OfferDown();
    }

    public void OnClickOfferUp()
    {
       // AppLogger.Log("OnClickOfferUp");
        _hireView.OfferUp();
    }

    private void OnClickStaff(string staffId)
    {
        //AppLogger.Log("OnClickStaff " + staffId);

        var staff = StaffModule.Instance.GetArchiveData(staffId);
        if (staff.IsAssigned()) {
           // AppLogger.Log("remove");
            StaffModule.Instance.RemoveBuilding(staffId);

            _listView.Reset();
            SetStaffCount();
        } else {
            if (staff.rarity == StaffRarity.NORMAL) {
                FireNormal(staffId);
            } else {
                FireRare(staffId);
            }
        }
    }

    public void OnClickShop()
    {
        ShopUICtrl.Open(ShopUICtrl.Section.Gacha);
    }

    private void FireNormal(string staffId)
    {
        UIMgr.Instance.OpenConfirmOkCancel(StaffText.CautionText.Locale(), StaffText.FireStaffBodyMsg.Locale(), StaffText.Fire.Locale(), StaffText.Back.Locale(),
            () => {
                StaffModule.Instance.FireStaff(staffId);

                _listView.Reset();
                SetStaffCount();
            });
    }

    private void FireRare(string staffId)
    {
        var staff = StaffModule.Instance.GetArchiveData(staffId);
        var get = 0;
        if (staff.rarity == StaffRarity.RARE) {
            get = StaffDefine.CouponRare;
        } else if (staff.rarity == StaffRarity.SR) {
            get = StaffDefine.CouponSR;
        }
        UIMgr.Instance.OpenConfirmOkCancel(
            StaffText.CautionText.Locale(),
            string.Format(StaffText.FireStaffBodyRareMsg.Locale(), get),
            StaffText.Fire.Locale(), StaffText.Back.Locale(),
            () => {
                StaffModule.Instance.FireStaff(staffId);
                PlayerModule.Instance.AddCoupon(get);

                _listView.Reset();
                SetStaffCount();
            });
    }

    private void SetStaffCount()
    {
        foreach (var countText in _staffCountTabTextList) {
            countText.text = string.Format("{0} {1}/{2}", "STAFF".Locale(), StaffModule.Instance.GetStaffCount(), StaffModule.Instance.GetStaffMaxCount());
        }
    }
}
