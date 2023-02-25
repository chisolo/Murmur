using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;

public class StaffDetail : PopupUIBaseCtrl
{
    public class StaffDetailArgs : PopupUIArgs
    {
        public string staffId;
    }

    public static string PrefabPath = "Assets/Res/UI/Prefab/Popup/StaffDetail.prefab";

    [SerializeField]
    private StaffItem _staffItem;


    public override void Init(PopupUIArgs arg)
    {
        var param = (StaffDetailArgs)arg;
        var staffData = StaffModule.Instance.GetArchiveData(param.staffId);

        _staffItem.Init(staffData);
    }

}