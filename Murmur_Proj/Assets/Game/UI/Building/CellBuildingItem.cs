using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;

public class CellBuildingItem : MonoBehaviour
{
    [SerializeField] GameObject _builtBg;
    [SerializeField] GameObject _unbuiltBg;
    [SerializeField] Text _cellNameTxt;
    [SerializeField] Image _portraitIcon;

    [SerializeField] GameObject _normalContent;
    [SerializeField] Text _levelTxt;
    [SerializeField] Button _improveBtn;
    [SerializeField] GameObject _alert;
    [SerializeField] Image _enhanceProgress;
    [SerializeField] Transform _statContent;
    [SerializeField] GameObject _statItemPrefab;

    [SerializeField] GameObject _unbuiltContent;
    [SerializeField] Button _buildBtn;
    [SerializeField] Text _buildCostTxt;
    [SerializeField] Text _buildTimeTxt;

    [SerializeField] GameObject _levelupContent;
    [SerializeField] AdTimeProgress _adTimerProgress;

    private BuildingData _buildingData;
    private Dictionary<int, BuildingStatItem> _statItems;

    public void Init(BuildingData buildingData)
    {
        _buildingData = buildingData;
        _statItems = new Dictionary<int, BuildingStatItem>();
    }
    public void Start()
    {
        _improveBtn.onClick.AddListener(OnImproveBtn);
        _buildBtn.onClick.AddListener(OnBuildBtn);

        Refresh();
    }
    public void Refresh()
    {
        RefreshInfo();
        if(_buildingData.Status == BuildingStatus.Normal) {
            RefreshNormalContent();
        } else if(_buildingData.Status == BuildingStatus.Unbuilt){
            RefreshUnBuiltContent();
        } else if(_buildingData.Status == BuildingStatus.Progress){
            RefreshLevelupContent();
        } 
    }
    private void RefreshMoney()
    {
        if(_buildingData.Status == BuildingStatus.Unbuilt){
            RefreshUnBuiltContent();
        } 
    }
    private void RefreshInfo()
    {
        if(_buildingData.GetLevel() <= 0) {
            _builtBg.SetActive(false);
            _unbuiltBg.SetActive(true);
            _portraitIcon.ShowSprite(GameUtil.NullWorkerIcon);
            _cellNameTxt.text = GameUtil.NullWorkerName.Locale();
        } else {
            _builtBg.SetActive(true);
            _unbuiltBg.SetActive(false);
            _portraitIcon.ShowSprite(string.Format(GameUtil.ResWorkerIconPath, _buildingData.Config.small_icon));
            _cellNameTxt.text = _buildingData.Config.name.Locale();
        }
    }
    private void RefreshNormalContent()
    {
        _normalContent.SetActive(true);
        _unbuiltContent.SetActive(false);
        _levelupContent.SetActive(false);
        _levelTxt.text = string.Format("{0} {1}", "LEVEL".Locale(), _buildingData.GetLevel());
        _alert.SetActive(_buildingData.IsEnhanceLevelRatioMax() && !_buildingData.IsMaxLevel());
        _enhanceProgress.fillAmount = _buildingData.EnhanceLevelRatio();
        _statContent.gameObject.SetActive(true);
        if(_statItems.Count > 0) {
            foreach(var statItem in _statItems.Values) {
                statItem.Refresh();
            }
        } else {
            if(GameUtil.BuildingServiceStats.TryGetValue(_buildingData.Config.service, out var stats)) {
                foreach(var stat in stats) {
                    var statItem = GameObject.Instantiate(_statItemPrefab, _statContent).GetComponent<BuildingStatItem>();
                    statItem.Init(_buildingData, stat, true);
                    statItem.transform.SetAsLastSibling();
                    _statItems.Add((int)stat, statItem);
                }
            }
        }
    }
    private void RefreshUnBuiltContent()
    {
        _normalContent.SetActive(false);
        _unbuiltContent.SetActive(true);
        _levelupContent.SetActive(false);
        int cost = (int)_buildingData.GetProp(BuildingProperty.LvlUpCost);
        bool enough = PlayerModule.Instance.Money >= cost;
        _buildCostTxt.text = FormatUtil.Currency(cost);
        _buildTimeTxt.text = FormatUtil.FormatTimeAuto((int)_buildingData.GetProp(BuildingProperty.LvlUpDuration));

        _buildBtn.interactable = _buildingData.CanLevelup() && enough;
    }
    private void RefreshLevelupContent()
    {
        _normalContent.SetActive(false);
        _unbuiltContent.SetActive(false);
        _levelupContent.SetActive(true);
        _adTimerProgress.Init(_buildingData);
    }
    private void OnImproveBtn()
    {
        BuildingMgr.Instance.PopupBuildingView(_buildingData.Config.id, false);
    }
    private void OnBuildBtn()
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
}
