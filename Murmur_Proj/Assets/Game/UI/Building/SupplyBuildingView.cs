using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;
using DG.Tweening;

public class SupplyBuildingView : PopupUIBaseCtrl
{
    public class Args : PopupUIArgs
    {
        public string buildingId;
    }
    public static string PrefabPath = "Assets/Res/UI/Prefab/Building/SupplyBuildingView.prefab";

    [SerializeField] Image _bigIcon;
    [SerializeField] Image _smallIcon;
    [SerializeField] Text _buildingNameTxt;
    [SerializeField] Text _buildingLevelTxt;
    [SerializeField] Text _buildingDescTxt;
    [SerializeField] Text _capacityTxt;
    [SerializeField] Image _previewIcon;

    [SerializeField] GameObject _normalContent;
    [SerializeField] Text _nextLevelTxt;
    [SerializeField] Text _nextCapacityTxt;
    [SerializeField] Button _preBtn;
    [SerializeField] Text _preTxt;
    [SerializeField] Button _solveBtn;
    [SerializeField] GameObject _levelUpBtnContent;
    [SerializeField] Button _levelUpBtn;
    [SerializeField] Text _levelUpCostTxt;
    [SerializeField] Text _levelUpTimeTxt;

    [SerializeField] GameObject _levelUpContent;
    [SerializeField] AdTimeProgress _adTimerProgress;

    [SerializeField] GameObject _maxContent;

    private BuildingData _buildingData;
    private BuildingRequire _preBuilding;
    private BuildingRequire _preTalent;
    public override void Init(PopupUIArgs arg)
    {
        _buildingData = BuildingModule.Instance.GetBuilding(((Args)arg).buildingId);

        EventModule.Instance.Register(EventDefine.BuildingLevelup, OnBuildingLevelupEvent);
        EventModule.Instance.Register(EventDefine.BuildingLevelupFinish, OnBuildingLevelupFinishEvent);
        EventModule.Instance.Register(EventDefine.UpdateItem, OnUpdateItemEvent);
    }
    void Start()
    {
        _bigIcon.ShowSprite(string.Format(GameUtil.ResBuildingIconPath, _buildingData.Config.big_icon));
        _smallIcon.ShowSprite(string.Format(GameUtil.ResBuildingIconPath, _buildingData.Config.small_icon));
        _buildingNameTxt.text = _buildingData.Config.name.Locale();
        _buildingDescTxt.text = _buildingData.Config.desc.Locale();
        _previewIcon.ShowSprite(string.Format(GameUtil.ResBuildingIconPath, _buildingData.Config.preview_icon));

        RefreshBuildingContent();

        _preBtn.onClick.AddListener(OnPreBtn);
        _levelUpBtn.onClick.AddListener(OnLevelupBtn);
        _solveBtn.onClick.AddListener(OnPreBtn);
    }
    void OnDestroy()
    {
        EventModule.Instance?.UnRegister(EventDefine.BuildingLevelup, OnBuildingLevelupEvent);
        EventModule.Instance?.UnRegister(EventDefine.BuildingLevelupFinish, OnBuildingLevelupFinishEvent);
        EventModule.Instance?.UnRegister(EventDefine.UpdateItem, OnUpdateItemEvent);
    }
    private void RefreshBuildingContent()
    {
        _buildingLevelTxt.text = string.Format("{0} {1}", "LEVEL".Locale(), _buildingData.GetLevel());
        _capacityTxt.text = FormatUtil.Currency((int)_buildingData.GetProp(BuildingProperty.Capacity), false);

        if(_buildingData.Status == BuildingStatus.Normal) {
            if(_buildingData.IsMaxLevel()) {
                RefreshMaxContent();
            } else {
                RefreshNormalContent();
            }
        } else if(_buildingData.Status == BuildingStatus.Progress) {
            RefreshLevelupContent();
        } else {
            Debug.Log("[BuidlingBuildView] We are in big trouble! status error = " + _buildingData.Status + ", Id = " + _buildingData.Config.id);
        }
    }
    private void RefreshPreContent()
    {
        _preBuilding = _buildingData.GetPreBuilding();
        _preTalent = _buildingData.GetPreTalent();
        var cost = (int)_buildingData.GetProp(BuildingProperty.LvlUpCost);
        if(_preBuilding != null) {
            _preBtn.gameObject.SetActive(true);
            _preTxt.text = LocaleModule.Instance.GetLocale("BUILDING_REQUIRE", BuildingModule.Instance.GetBuildingName(_preBuilding.id).Locale(), _preBuilding.level);
        } else if(_preTalent != null) {
            _preBtn.gameObject.SetActive(true);
            _preTxt.text = LocaleModule.Instance.GetLocale("TALENT_REQUIRE", TalentModule.Instance.GetTalentName(_preTalent.id).Locale());
        } else if(cost > PlayerModule.Instance.MoneyLimit) {
            _preBtn.gameObject.SetActive(true);
            _preTxt.text = "CAPACITY_REQUIRE".Locale(FormatUtil.Currency(cost, false));
        } else {
            _preBtn.gameObject.SetActive(false);
        }
    }
    private void RefreshNormalContent()
    {
        _normalContent.SetActive(true);
        _levelUpContent.SetActive(false);
        _maxContent.SetActive(false);
        _nextLevelTxt.text = string.Format("{0} {1}", "LEVEL".Locale(), _buildingData.GetLevel() + 1);
        _nextCapacityTxt.text = FormatUtil.Currency((int)_buildingData.GetNextProp(BuildingProperty.Capacity), false);
        RefreshPreContent();
        if(_preBuilding == null && _preTalent == null) {
            _solveBtn.gameObject.SetActive(false);
            _levelUpBtnContent.SetActive(true);
            _levelUpTimeTxt.text = FormatUtil.FormatTimeAuto((int)_buildingData.GetProp(BuildingProperty.LvlUpDuration));
            var cost = (int)_buildingData.GetProp(BuildingProperty.LvlUpCost);
            _levelUpCostTxt.text = FormatUtil.Currency(cost);
            _levelUpBtn.interactable = _buildingData.CanLevelup() && (PlayerModule.Instance.Money >= cost);
        } else {
            _solveBtn.gameObject.SetActive(true);
            _levelUpBtnContent.SetActive(false);
        }
    }
    private void RefreshLevelupContent()
    {
        _normalContent.SetActive(false);
        _levelUpContent.SetActive(true);
        _maxContent.SetActive(false);

        _adTimerProgress.Init(_buildingData);
    }
    private void RefreshMaxContent()
    {
        _normalContent.SetActive(false);
        _levelUpContent.SetActive(false);
        _maxContent.SetActive(true);
    }
    private void OnPreBtn()
    {
        if(_preBuilding != null) {
            HideForce();
            BuildingMgr.Instance.PopupBuildingView(_preBuilding.id, true);
        } else if(_preTalent != null) {
            HideForce();
            var arg = new TalentUICtrl.Args(_preTalent.id);
            UIMgr.Instance.OpenUIByClick(TalentUICtrl.PrefabPath, arg, false, true);
        } else {
            HideForce();
            BuildingMgr.Instance.PopupBuildingView(GameUtil.TreasuryBuildingId, true);
        }
    }
    private void OnLevelupBtn()
    {
        if(_buildingData.CanLevelup()) {
            if (BuildingModule.Instance.WorkTeam.HasEmpty()) {
                BuildingModule.Instance.LevelupBuilding(_buildingData.Config.id);
            } else {
                var arg = new TeamBusyPopupCtrl.Args();
                arg.title = GameUtil.BuildingTeamFullTitleText.Locale();
                arg.body = GameUtil.BuildingTeamFullBodyText.Locale();
                arg.shopText = GameUtil.BuildingTeamFullShopText.Locale();
                arg.teamMax = BuildingModule.Instance.WorkTeam.IsTeamMax();
                arg.timeData = BuildingModule.Instance.GetProcessBuildlingData();
                UIMgr.Instance.OpenUIByClick(TeamBusyPopupCtrl.PrefabPath, arg, false, false);
            }
        }
    }
    private void OnBuildingLevelupEvent(Component sender, EventArgs e)
    {
        BuildingLevelupEventArgs args = e as BuildingLevelupEventArgs;
        if(_buildingData.Config.id == args.buildingId) {
            RefreshBuildingContent();
        }
    }
    private void OnBuildingLevelupFinishEvent(Component sender, EventArgs e)
    {
        BuildingLevelupEventArgs args = e as BuildingLevelupEventArgs;
        if(_buildingData.Config.id == args.buildingId) {
            RefreshBuildingContent();
        }
    }
    private void OnUpdateItemEvent(Component sender, EventArgs e)
    {
        UpdateItemArgs arg = e as UpdateItemArgs;
        if(arg.item == ItemType.Money && _buildingData.Status == BuildingStatus.Normal && !_buildingData.IsMaxLevel()) {
            RefreshNormalContent();
        }
    }
}
