using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;
using DG.Tweening;

public class CellBuildingView : PopupUIBaseCtrl
{
    public class CellBuildingViewArgs : PopupUIArgs
    {
        public string buildingId;
    }
    public static string PrefabPath = "Assets/Res/UI/Prefab/Building/CellBuildingView.prefab";

    [SerializeField] Image _bigIcon;
    [SerializeField] Image _smallIcon;
    [SerializeField] Text _buildingNameTxt;
    [SerializeField] Transform _statContentTrans;
    [SerializeField] GameObject _statItemPrefab;

    [SerializeField] GameObject _normalContent;
    [SerializeField] Text _normalLevelTxt;
    [SerializeField] Image _enhanceProgressBar;
    [SerializeField] List<ToggleUI> _stars;
    [SerializeField] List<ToggleGo> _bonus;
    [SerializeField] List<Image> _bonusIcons;
    [SerializeField] List<Text> _bonusTxts;

    [SerializeField] GameObject _readyContent;
    [SerializeField] GameObject _levelUpBtnContent;
    [SerializeField] Button _preBtn;
    [SerializeField] Button _solveBtn;
    [SerializeField] Text _preTxt;
    [SerializeField] Text _readyLevelTxt;
    [SerializeField] Text _readyNextLevelTxt;
    [SerializeField] Button _levelUpBtn;
    [SerializeField] Text _levelUpCostTxt;
    [SerializeField] Text _readyTimeTxt;

    [SerializeField] GameObject _levelupContent;
    [SerializeField] Text _levelupLevelTxt;
    [SerializeField] AdTimeProgress _adTimerProgress;

    [SerializeField] RectTransform _enhanceContent;
    [SerializeField] Text _enhanceLevelText;
    [SerializeField] Transform _enhanceScrollContent;
    [SerializeField] GameObject _enhancePrefab;
    [SerializeField] ScrollRect _enhanceScroll;

    private BuildingData _buildingData;
    private BuildingRequire _preBuilding;
    private BuildingRequire _preTalent;
    private Dictionary<int, BuildingStatItem> _statItems = new Dictionary<int, BuildingStatItem>();
    private Dictionary<string, BuildingEnhanceItem> _enhanceItems = new Dictionary<string, BuildingEnhanceItem>();


    public override void Init(PopupUIArgs arg)
    {
        _buildingData = BuildingModule.Instance.GetBuilding(((CellBuildingViewArgs)arg).buildingId);

        EventModule.Instance.Register(EventDefine.BuildingEnhanceLevelup, OnBuildingEnhanceLevelupEvent);
        EventModule.Instance.Register(EventDefine.BuildingLevelup, OnBuildingLevelupEvent);
        EventModule.Instance.Register(EventDefine.BuildingLevelupFinish, OnBuildingLevelupFinishEvent);
        EventModule.Instance.Register(EventDefine.UpdateItem, OnUpdateItemEvent);

        _preBtn.onClick.AddListener(OnPreBtn);
        _solveBtn.onClick.AddListener(OnPreBtn);
        _levelUpBtn.onClick.AddListener(OnLevelupBtn);
    }
    void Start()
    {
        _smallIcon.ShowSprite(string.Format(GameUtil.ResWorkerIconPath, _buildingData.Config.small_icon));
        _bigIcon.ShowSprite(string.Format(GameUtil.ResBuildingIconPath, _buildingData.Config.big_icon));
        _buildingNameTxt.text = _buildingData.Config.name.Locale();

        RefreshBonusContent();
        RefreshCellStat();
        RefreshCellInfo();
        RefreshEnhanceItems();
    }
    void OnDestroy()
    {
        EventModule.Instance?.UnRegister(EventDefine.BuildingEnhanceLevelup, OnBuildingEnhanceLevelupEvent);
        EventModule.Instance?.UnRegister(EventDefine.BuildingLevelup, OnBuildingLevelupEvent);
        EventModule.Instance?.UnRegister(EventDefine.BuildingLevelupFinish, OnBuildingLevelupFinishEvent);
        EventModule.Instance?.UnRegister(EventDefine.UpdateItem, OnUpdateItemEvent);
    }
    private void RefreshCellStat()
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
    private void RefreshCellInfo(bool anim  = false)
    {
        if(_buildingData.Status == BuildingStatus.Normal) {
            if(_buildingData.IsEnhanceLevelRatioMax() && !_buildingData.IsMaxLevel()) {
                RefreshReadyContent();
            } else {
                RefreshNormalContent(anim);
            }
        } else {
            RefreshLevelupContent();
        } 
    }
    private void RefreshBonusContent()
    {
        var setBonus = _buildingData.GetEnhanceSetBonus();
        for(int i = 0; i < setBonus.Count; ++i) {
            _bonusIcons[i].ShowSprite(GameUtil.ItemIcons[setBonus[i].item]);
            _bonusTxts[i].text = setBonus[i].amount.ToString();
        }
    }
    private void RefreshNormalContent(bool anim)
    {
        _normalContent.SetActive(true);
        _readyContent.SetActive(false);
        _levelupContent.SetActive(false);
        _enhanceContent.sizeDelta = new Vector2(1130, 1171.6f);
        _normalLevelTxt.text = _buildingData.GetLevel().ToString();
        RefreshEnhanceLevelRatio(anim);
    }

    private void RefreshReadyContent()
    {
        _normalContent.SetActive(false);
        _readyContent.SetActive(true);
        _levelupContent.SetActive(false);
        _enhanceContent.sizeDelta = new Vector2(1130, 1104.6f);

        RefreshPreContent();

        if(_preBuilding == null && _preTalent == null) {
            _levelUpBtnContent.SetActive(true);
            _solveBtn.gameObject.SetActive(false);
            _readyLevelTxt.text = _buildingData.GetLevel().ToString();
            _readyNextLevelTxt.text = (_buildingData.GetLevel() + 1).ToString();
            var cost = (int)_buildingData.GetProp(BuildingProperty.LvlUpCost);
            _levelUpCostTxt.text = FormatUtil.Currency(cost);
            _readyTimeTxt.text = FormatUtil.FormatTimeAuto((int)_buildingData.GetProp(BuildingProperty.LvlUpDuration));
            _levelUpBtn.interactable = _buildingData.CanLevelup() && (PlayerModule.Instance.Money >= cost);
        } else {
            _levelUpBtnContent.SetActive(false);
            _solveBtn.gameObject.SetActive(true);
        }
    }

    private void RefreshLevelupContent()
    {
        _normalContent.SetActive(false);
        _readyContent.SetActive(false);
        _levelupContent.SetActive(true);
        _enhanceContent.sizeDelta = new Vector2(1130, 931.6f);
        _levelupLevelTxt.text = _buildingData.GetLevel().ToString();
        _adTimerProgress.Init(_buildingData);
    }
    private void RefreshEnhanceItems()
    {
        _enhanceLevelText.text = string.Format("{0} {1}", "LEVEL".Locale(), _buildingData.GetLevel());

        int childCount = _enhanceScrollContent.childCount;
        for(int i = 0; i < childCount; ++i) {
            DestroyImmediate(_enhanceScrollContent.GetChild(0).gameObject);
        }
        _enhanceItems.Clear();
        var curEnhances = _buildingData.GetCurEnhances();
        foreach(var enhance in curEnhances.Values) {
            var enhanceItem = GameObject.Instantiate(_enhancePrefab, _enhanceScrollContent).GetComponent<BuildingEnhanceItem>();
            enhanceItem.Init(_buildingData, enhance);
            enhanceItem.transform.SetAsLastSibling();
            _enhanceItems.Add(enhance.id, enhanceItem);
        }
        _enhanceScroll.verticalNormalizedPosition = 1;
    }
    private void RefreshEnhanceItem(string enhanceId)
    {
        if(_enhanceItems.TryGetValue(enhanceId, out var enhanceItem)) {
            enhanceItem.Refresh();
        }
    }
    private void RefreshEnhanceLevelRatio(bool anim)
    {
        var ratio = _buildingData.EnhanceLevelRatio();
        if(anim) _enhanceProgressBar.DOFillAmount(ratio,0.1f);
        else _enhanceProgressBar.fillAmount = ratio;
        if(ratio >= 1.0) {
            _stars[0].Toggle(1);
            _stars[1].Toggle(1);
            _stars[2].Toggle(1);
            _bonus[0].Toggle(1);
            _bonus[1].Toggle(1);
            _bonus[2].Toggle(1);
        } else if(ratio >= 0.6) {
            _stars[0].Toggle(1);
            _stars[1].Toggle(1);
            _stars[2].Toggle(0);
            _bonus[0].Toggle(1);
            _bonus[1].Toggle(1);
            _bonus[2].Toggle(0);
        } else if(ratio >= 0.3) {
            _stars[0].Toggle(1);
            _stars[1].Toggle(0);
            _stars[2].Toggle(0);
            _bonus[0].Toggle(1);
            _bonus[1].Toggle(0);
            _bonus[2].Toggle(0);
        } else {
            _stars[0].Toggle(0);
            _stars[1].Toggle(0);
            _stars[2].Toggle(0);
            _bonus[0].Toggle(0);
            _bonus[1].Toggle(0);
            _bonus[2].Toggle(0);
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

    private void OnBuildingEnhanceLevelupEvent(Component sender, EventArgs e)
    {
        BuildingLevelupEventArgs args = e as BuildingLevelupEventArgs;
        if(_buildingData.Config.id != args.buildingId) return;
        RefreshEnhanceItem(args.enhanceId);
        RefreshCellStat();
        RefreshCellInfo(true);
    }
    private void OnBuildingLevelupEvent(Component sender, EventArgs e)
    {
        BuildingLevelupEventArgs args = e as BuildingLevelupEventArgs;
        if(_buildingData.Config.id != args.buildingId) return;
        RefreshCellInfo();
    }
    private void OnBuildingLevelupFinishEvent(Component sender, EventArgs e)
    {
        BuildingLevelupEventArgs args = e as BuildingLevelupEventArgs;
        if(_buildingData.Config.id == args.buildingId) {
            RefreshBonusContent();
            RefreshCellStat();
            RefreshCellInfo();
            RefreshEnhanceItems();
        } else if(_buildingData.Status == BuildingStatus.Normal) {
            RefreshReadyContent();
        }
    }
    private void OnUpdateItemEvent(Component sender, EventArgs e)
    {
        UpdateItemArgs arg = e as UpdateItemArgs;
        if(arg.item == ItemType.Money && _buildingData.Status == BuildingStatus.Normal && _buildingData.IsEnhanceLevelRatioMax() && !_buildingData.IsMaxLevel()) {
            RefreshReadyContent();
        }
        foreach(var enhanceItem in _enhanceItems.Values) {
            enhanceItem.Refresh();
        }
    }
}
