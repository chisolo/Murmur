using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class StaffListView : MonoBehaviour
{
    [SerializeField]
    private GameObject _noOfficeNoStaffMsg;

    [SerializeField]
    private GameObject _assignedOfficeHeader;
    [SerializeField]
    private GameObject _assignedOfficeNoStaffMsg;

    [SerializeField]
    private RectTransform _noOfficeContent;

    [SerializeField]
    private RectTransform _assignedOfficeContent;

    [SerializeField]
    private StaffItem _staffItemPrefab;

    [SerializeField]
    private ScrollRect _scroll;

    public System.Action<string> onClickStaffAction;
    private StaffData _staffData;

    private void OnEnable()
    {
        //AppLogger.Log("list OnEnable");
        Reset();

        TaskModule.Instance.NextFrame(() => {
            LayoutRebuilder.MarkLayoutForRebuild(_noOfficeContent);
            LayoutRebuilder.MarkLayoutForRebuild(_scroll.content);
        });
    }

    public void Init(StaffData staffData)
    {
        _staffData = staffData;
        RefreshList();
    }

    public void Reset()
    {
        if (_staffData == null) {
            return;
        }
        if (!_staffData.isDirty) {
            // no change
            return;
        }

        foreach (Transform child in _noOfficeContent) {
            Destroy(child.gameObject);
        }

        foreach (Transform child in _assignedOfficeContent) {
            Destroy(child.gameObject);
        }

        RefreshList();
        _staffData.isDirty = false;
    }

    public void RefreshList()
    {
        //AppLogger.Log("staff list RefreshList");
        var noOfficeList = _staffData.GetNoOfficeStaffList();
        _noOfficeNoStaffMsg.SetActive(noOfficeList.Count == 0);

        foreach (var staff in noOfficeList) {
            var go = Instantiate(_staffItemPrefab, _noOfficeContent);
            go.Init(staff);
            go.OnClickSelfAction = OnClickStaff;
        }

        var assignedList = _staffData.GetAssignedOfficeStaffList();
        _assignedOfficeNoStaffMsg.SetActive(assignedList.Count == 0);

        foreach (var staff in assignedList) {
            var go = Instantiate(_staffItemPrefab, _assignedOfficeContent);
            go.Init(staff);
            go.OnClickSelfAction = OnClickStaff;
        }

    }

    private void OnClickStaff(string staffId)
    {
        onClickStaffAction?.Invoke(staffId);
    }

}