using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;

public class DivisionBuildingView : PopupUIBaseCtrl
{
    public class DivisionBuildingViewArgs : PopupUIArgs
    {
        public string buildingId;
    }
    public static string PrefabPath = "Assets/Res/UI/Prefab/Building/DivisionBuildingView.prefab";

    [SerializeField] Image _bigIcon;
    [SerializeField] Image _smallIcon;
    [SerializeField] Text _buildingNameTxt;
    [SerializeField] Text _buildingLevelTxt;
    [SerializeField] Text _buildingDescTxt;

    [SerializeField] Button _preBtn;
    [SerializeField] Text _preTxt;

    [SerializeField] GameObject _loadingImageObj;
    [SerializeField] GameObject _levelUpBtnContent;
    [SerializeField] GameObject _maxImageObj;
    [SerializeField] Text _loadingTxt;
    [SerializeField] Text _levelUpTimerText;
    [SerializeField] Button _levelUpBtn;
    [SerializeField] Text _levelUpCostTxt;
    [SerializeField] Button _solveBtn;

    [SerializeField] Transform _statContentTrans;
    [SerializeField] GameObject _statItemPrefab;

    [SerializeField] GameObject _levelUpContent;
    [SerializeField] AdTimeProgress _adTimerProgress;

    [SerializeField] GameObject _cellContent;
    [SerializeField] Transform _cellContentTrans;
    [SerializeField] GameObject _cellItemPrefab;

    private BuildingData _buildingData;
    private BuildingRequire _preBuilding;
    private BuildingRequire _preTalent;
    private Dictionary<int, BuildingStatItem> _statItems = new Dictionary<int, BuildingStatItem>();
    private Dictionary<string, CellBuildingItem> _cellItems = new Dictionary<string, CellBuildingItem>();

    public override void Init(PopupUIArgs arg)
    {
        _buildingData = BuildingModule.Instance.GetBuilding(((DivisionBuildingViewArgs)arg).buildingId);

        EventModule.Instance.Register(EventDefine.BuildingLevelup, OnBuildingLevelupEvent);
        EventModule.Instance.Register(EventDefine.BuildingLevelupFinish, OnBuildingLevelupFinishEvent);
        EventModule.Instance.Register(EventDefine.BuildingEnhanceLevelup, OnBuildingEnhanceLevelupEvent);
        EventModule.Instance.Register(EventDefine.UpdateItem, OnUpdateItemEvent);
    }
    void Start()
    {
        _bigIcon.ShowSprite(string.Format(GameUtil.ResBuildingIconPath, _buildingData.Config.big_icon));
        _smallIcon.ShowSprite(string.Format(GameUtil.ResBuildingIconPath, _buildingData.Config.small_icon));
        _buildingNameTxt.text = _buildingData.Config.name.Locale();
        _buildingDescTxt.text = _buildingData.Config.desc.Locale();

        RefreshBuildingContent();

        _preBtn.onClick.AddListener(OnPreBtn);
        _levelUpBtn.onClick.AddListener(OnLevelupBtn);
        _solveBtn.onClick.AddListener(OnPreBtn);
    }
    void OnDestroy()
    {
        EventModule.Instance?.UnRegister(EventDefine.BuildingEnhanceLevelup, OnBuildingEnhanceLevelupEvent);
        EventModule.Instance?.UnRegister(EventDefine.BuildingLevelup, OnBuildingLevelupEvent);
        EventModule.Instance?.UnRegister(EventDefine.BuildingLevelupFinish, OnBuildingLevelupFinishEvent);
        EventModule.Instance?.UnRegister(EventDefine.UpdateItem, OnUpdateItemEvent);
    }
    private void RefreshBuildingContent()
    {
        _buildingLevelTxt.text = string.Format("{0} {1}", "LEVEL".Locale(), _buildingData.GetLevel());
        RefreshPreContent();
        RefreshStats();
        RefreshNormal();
        RefreshLevelup();

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
    private void RefreshStats()
    {
        if(_statItems.Count > 0) {
            foreach(var statItem in _statItems.Values) {
                statItem.Refresh();
            }
        } else {
            if(GameUtil.BuildingServiceStats.TryGetValue(_buildingData.Config.service, out var stats)) {
                foreach(var stat in stats) {
                    var statItem = GameObject.Instantiate(_statItemPrefab, _statContentTrans).GetComponent<BuildingStatItem>();
                    statItem.Init(_buildingData, stat);
                    statItem.transform.SetAsLastSibling();
                    _statItems.Add((int)stat, statItem);
                }
            }
        }
    }
    private void RefreshNormal()
    {
        if(_buildingData.IsMaxLevel()) {
            _levelUpBtn.gameObject.SetActive(false);
            _levelUpTimerText.gameObject.SetActive(false);
            _levelUpBtnContent.SetActive(true);
            _solveBtn.gameObject.SetActive(false);
            _loadingImageObj.SetActive(false);
            _maxImageObj.SetActive(true);
            _loadingTxt.gameObject.SetActive(true);
            _loadingTxt.text = "Max";
        } else {
            if(_preBuilding == null && _preTalent == null) {
                _levelUpBtnContent.SetActive(true);
                _solveBtn.gameObject.SetActive(false);
                if(_buildingData.Status == BuildingStatus.Normal) {
                    _levelUpBtn.gameObject.SetActive(true);
                    _levelUpTimerText.gameObject.SetActive(true);
                    _levelUpTimerText.text = FormatUtil.FormatTimeAuto((int)_buildingData.GetProp(BuildingProperty.LvlUpDuration));
                    var cost = (int)_buildingData.GetProp(BuildingProperty.LvlUpCost);
                    _levelUpCostTxt.text = FormatUtil.Currency(cost);
                    _loadingImageObj.SetActive(false);
                    _loadingTxt.gameObject.SetActive(false);
                    _maxImageObj.SetActive(false);
                    _levelUpBtn.interactable = _buildingData.CanLevelup() && (PlayerModule.Instance.Money >= cost);
                } else if(_buildingData.Status == BuildingStatus.Progress) {
                    _levelUpBtn.gameObject.SetActive(false);
                    _levelUpTimerText.gameObject.SetActive(false);
                    _loadingImageObj.SetActive(true);
                    _loadingTxt.gameObject.SetActive(true);
                    _loadingTxt.text = "LEVEL_UP_PROGRESS".Locale();
                    _maxImageObj.SetActive(false);
                }
            } else {
                _levelUpBtnContent.SetActive(false);
                _solveBtn.gameObject.SetActive(true);
            }

        }
    }
    private void RefreshLevelup()
    {
        if(_buildingData.Status == BuildingStatus.Normal) {
            _levelUpContent.SetActive(false);
            _cellContent.SetActive(true);
            var childCount = _cellContentTrans.childCount;
            for(int i = 0; i < childCount; ++i) {
                DestroyImmediate(_cellContentTrans.transform.GetChild(0).gameObject);
            }
            _cellItems.Clear();
            var curLevel = _buildingData.GetLevel();
            for(int i = 0; i < curLevel; ++i) {
                var cellData = _buildingData.GetChildData(i);
                if(cellData != null) {
                    var cellItem = GameObject.Instantiate(_cellItemPrefab, _cellContentTrans).GetComponent<CellBuildingItem>();
                    cellItem.Init(cellData);
                    cellItem.transform.SetAsLastSibling();
                    _cellItems.Add(cellData.Config.id, cellItem);
                }
            }
        } else {
            _levelUpContent.SetActive(true);
            _cellContent.SetActive(false);
            _adTimerProgress.Init(_buildingData);
        }
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
        if(args.buildingId == _buildingData.Config.id) {
            RefreshNormal();
            RefreshLevelup();
        }
        if(_cellItems.TryGetValue(args.buildingId, out var cell)) {
            cell.Refresh();
            RefreshStats();
        }
    }
    private void OnBuildingLevelupFinishEvent(Component sender, EventArgs e)
    {
        BuildingLevelupEventArgs args = e as BuildingLevelupEventArgs;
        if(args.buildingId == _buildingData.Config.id) {
            RefreshBuildingContent();
        } else if(_buildingData.Status == BuildingStatus.Normal) {
            RefreshPreContent();
            RefreshNormal();
        }
        if(_cellItems.TryGetValue(args.buildingId, out var cell)) {
            cell.Refresh();
            RefreshStats();
        }
    }
    private void OnBuildingEnhanceLevelupEvent(Component sender, EventArgs e)
    {
        BuildingLevelupEventArgs args = e as BuildingLevelupEventArgs;
        if(_cellItems.TryGetValue(args.buildingId, out var cell)) {
            cell.Refresh();
            RefreshStats();
        }
    }
    private void OnUpdateItemEvent(Component sender, EventArgs e)
    {
        UpdateItemArgs arg = e as UpdateItemArgs;
        if(arg.item == ItemType.Money) {
            RefreshNormal();
            foreach(var cellItem in _cellItems.Values) {
                cellItem.Refresh();
            }
        }
    }
}
