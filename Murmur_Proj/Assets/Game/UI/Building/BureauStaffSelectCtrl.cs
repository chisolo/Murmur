using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lemegeton;
using UnityEngine.UI;

public class BureauStaffSelectCtrl : PopupUIBaseCtrl
{
    public static string PrefabPath = "Assets/Res/UI/Prefab/Building/StaffSelect.prefab";
    public class BureauStaffSelectCtrlArgs : PopupUIArgs
    {
        public string buildingId;
    }

    [SerializeField]
    private StaffListView _listView;

    private string _buildingId;

    public override void Init(PopupUIArgs arg)
    {
        var param = (BureauStaffSelectCtrlArgs)arg;

        _buildingId = param.buildingId;
        var staffData = StaffModule.Instance.GetStaffData();
        _listView.Init(staffData);
        _listView.onClickStaffAction = OnClickStaff;
    }

    private void OnClickStaff(string staffId)
    {
        StaffModule.Instance.AssignBuilding(staffId, _buildingId);

        this.OnClickClose();
    }
}
