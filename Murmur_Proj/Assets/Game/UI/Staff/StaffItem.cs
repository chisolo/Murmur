using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;
using System;

public class StaffItem : MonoBehaviour
{
    [Header("头像")]
    [SerializeField]
    private Image _profileImg;
    [SerializeField]
    private Image _profileBg;
    [SerializeField]
    private Image _smileIcon;

    [SerializeField]
    private Text _nameLabel;

    [SerializeField]
    private Image _salaryBg;
    [SerializeField]
    private Text _salaryLabel;
    [SerializeField]
    private bool _salaryStrShow;

    [Header("品质")]
    [SerializeField]
    private Image _rarityBg;
    [SerializeField]
    private Text _rarityLabel;

    [SerializeField]
    private AttributeItem attributeItemPrefab;
    [SerializeField]
    private Transform attributeRoot;

    private StaffArchiveData _staffArchiveData;

    public Action<string> OnClickSelfAction;

    public void Init(StaffArchiveData staffArchiveData)
    {
        _staffArchiveData = staffArchiveData;

        // icon
        var puppetConfig = PuppetModule.Instance.GetStaffPuppet(staffArchiveData.puppetId);
        _profileImg.ShowSprite(AtlasDefine.GetStaffIconPath(puppetConfig.icon));

        _nameLabel.text = staffArchiveData.name.Locale();
        if (_rarityLabel) {
            _rarityLabel.text = staffArchiveData.rarity;
        }
        if (_rarityBg) {
            _rarityBg.ShowSprite(AtlasDefine.GetStaffRarityBgPath(staffArchiveData.rarity));
        }

        SetupSalary();
        if (_salaryBg) {
            _salaryBg.ShowSprite(AtlasDefine.GetStaffRarityBgPath(staffArchiveData.rarity));
        }

        if (_profileBg != null) {
            _profileBg.ShowSprite(AtlasDefine.GetStaffIconBgPath(staffArchiveData.rarity));
        }
        if (_smileIcon) {
            if (staffArchiveData.rarity == StaffRarity.NORMAL) {
                _smileIcon.gameObject.SetActive(false);
            } else {
                _smileIcon.gameObject.SetActive(true);
                _smileIcon.ShowSprite(AtlasDefine.GetStaffSmileIconPath(staffArchiveData.rarity));
            }
        }

        foreach (var attr in staffArchiveData.attributes) {
            var obj = Instantiate(attributeItemPrefab, attributeRoot);
            obj.Init(attr);
        }
    }

    public void ReInit(StaffArchiveData staffArchiveData)
    {
        Reset();
        Init(staffArchiveData);
    }
    public void Reset()
    {
        foreach (Transform child in attributeRoot) {
            Destroy(child.gameObject);
        }
    }

    public void SetupSalary()
    {
        if (_salaryLabel) {
            var salary = _staffArchiveData.salary;
            //Debug.Log("base salary " + salary);
            //Debug.Log("_staffArchiveData.buildingId " + _staffArchiveData.buildingId);
            if (!string.IsNullOrEmpty(_staffArchiveData.buildingId)) {
                var buildingData = BuildingModule.Instance.GetBuilding(_staffArchiveData.buildingId);
                salary = Mathf.FloorToInt(buildingData.GetProp(BuildingProperty.Salary));
            }

            if (_salaryStrShow) {
                _salaryLabel.text = string.Format("{0}:{1}/h", "SALARY".Locale(), salary);
            } else {
                _salaryLabel.text = string.Format("{0}/h", salary);
            }

        }
    }

    public void OnClickInfo()
    {
        //AppLogger.Log("OnClickInfo");

        var arg = new StaffDetail.StaffDetailArgs();
        arg.staffId = _staffArchiveData.id;
        UIMgr.Instance.OpenUIByClick(StaffDetail.PrefabPath, arg, true, false);
    }

    public void OnClickSelf()
    {
        //AppLogger.Log("OnClickSelf");

        OnClickSelfAction?.Invoke(_staffArchiveData.id);
    }

    private void OnDestroy()
    {
        OnClickSelfAction = null;
    }

#if UNITY_EDITOR
    [ContextMenu("SwitchInBureau")]
    public void SwitchInBureau()
    {
        var staffArchiveData = StaffModule.Instance.GetArchiveData(_staffArchiveData.id);
        if (string.IsNullOrEmpty(staffArchiveData.buildingId)) {
            staffArchiveData.buildingId = "1";
        } else {
            staffArchiveData.buildingId = "";
        }

        StaffModule.Instance.SaveStaff();
    }
#endif
}