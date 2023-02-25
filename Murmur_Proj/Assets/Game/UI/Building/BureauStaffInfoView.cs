using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BureauStaffInfoView : MonoBehaviour
{
    [SerializeField]
    private StaffItem _staffItem;

    [SerializeField]
    private GameObject _noStaffView;

    private BuildingData _buildingData;

    public void Start()
    {
        EventModule.Instance.Register(EventDefine.AssignBuildingStaff, OnAssignBuildingStaffEvent);
    }

    private void OnDestroy()
    {
        EventModule.Instance?.UnRegister(EventDefine.AssignBuildingStaff, OnAssignBuildingStaffEvent);
    }

    public void Init(BuildingData buildingData)
    {
        _buildingData = buildingData;
        var staffId = _buildingData.GetStaff();
        if (string.IsNullOrEmpty(staffId)) {
            // no staff
            _noStaffView.SetActive(true);
            _staffItem.gameObject.SetActive(false);
        } else {
            _noStaffView.SetActive(false);
            _staffItem.gameObject.SetActive(true);

            var staff = StaffModule.Instance.GetArchiveData(staffId);
            _staffItem.Init(staff);
        }

    }

    public void RefreshSalary()
    {
        var staffId = _buildingData.GetStaff();
        if (!string.IsNullOrEmpty(staffId)) {
            _staffItem.SetupSalary();
        }
    }

    public void OnClickAssign()
    {
        var staffData = StaffModule.Instance.GetStaffData();
        if (staffData.GetNoOfficeStaffList().Count == 0) {
            var arg = new StaffHireCtrl.StaffHireCtrlArgs();
            arg.buildingId = _buildingData.Config.id;
            UIMgr.Instance.OpenUIByClick(StaffHireCtrl.PrefabPath, arg, false, true);
        } else {
            var arg = new BureauStaffSelectCtrl.BureauStaffSelectCtrlArgs();
            arg.buildingId = _buildingData.Config.id;
            UIMgr.Instance.OpenUIByClick(BureauStaffSelectCtrl.PrefabPath, arg, true, false);
        }
    }

    public void OnClickReplace()
    {
        //AppLogger.Log("OnClickReplace");
        var arg = new BureauStaffSelectCtrl.BureauStaffSelectCtrlArgs();
        arg.buildingId = _buildingData.Config.id;
        UIMgr.Instance.OpenUIByClick(BureauStaffSelectCtrl.PrefabPath, arg, true, false);
    }

    public void OnClickRemove()
    {
        //AppLogger.Log("OnClickRemove");
        StaffModule.Instance.RemoveBuilding(_buildingData.GetStaff());

        _noStaffView.SetActive(true);
        _staffItem.gameObject.SetActive(false);
    }

    private void OnAssignBuildingStaffEvent(Component sender, System.EventArgs e)
    {
        //AppLogger.Log("OnAssignBuildingStaffEvent");
        BuildingStaffEventArgs args = e as BuildingStaffEventArgs;
        if (args.buildingId == _buildingData.Config.id && !string.IsNullOrEmpty(args.staffId)) {
            _noStaffView.SetActive(false);
            _staffItem.gameObject.SetActive(true);

            var staff = StaffModule.Instance.GetArchiveData(args.staffId);
            _staffItem.ReInit(staff);
        }
    }
}
