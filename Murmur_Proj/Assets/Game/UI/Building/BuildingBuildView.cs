using System;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;

public class BuildingBuildView : PopupUIBaseCtrl
{
    public class Args : PopupUIArgs
    {
        public string buildingId;
    }
    public static string PrefabPath = "Assets/Res/UI/Prefab/Building/BuildingBuildView.prefab";
    
    [SerializeField] Button _closeBtn;
    [SerializeField] Text _buildingNameTxt;
    [SerializeField] Text _buildingDescTxt;
    [SerializeField] Image _previewIcon;
    [SerializeField] SliceFillImage _progressBar;
    [SerializeField] Text _buildingTimeTxt;
    [SerializeField] Button _preBtn;
    [SerializeField] Text _preTxt;
    [SerializeField] GameObject _unBuildContent;
    [SerializeField] Button _buildingBtn;
    [SerializeField] Text _buildingCostTxt;
    [SerializeField] Button _solveBtn;
    [SerializeField] GameObject _inBuildingContent;
    [SerializeField] AdTimeProgress _adTimerProgress;


    private BuildingData _buildingData;
    private BuildingRequire _preBuilding;
    private BuildingRequire _preTalent;
    public override void Init(PopupUIArgs arg)
    {
        _buildingData = BuildingModule.Instance.GetBuilding(((Args)arg).buildingId);

        EventModule.Instance.Register(EventDefine.BuildingLevelup, OnBuildingLevelupEvent);
        EventModule.Instance.Register(EventDefine.BuildingLevelupFinish, OnBuildingLevelupFinishEvent);
        EventModule.Instance.Register(EventDefine.TalentResearchComplete, OnTalentResearchCompleteEvent);
        EventModule.Instance.Register(EventDefine.UpdateItem, OnUpdateItemEvent);
    }

    void Start()
    {
        _buildingNameTxt.text = _buildingData.Config.name.Locale();
        _buildingDescTxt.text = _buildingData.Config.desc.Locale();
        _previewIcon.ShowSprite(string.Format(GameUtil.ResBuildingIconPath, _buildingData.Config.preview_icon));

        RefreshBuildingContent();

        _closeBtn.onClick.AddListener(OnClickClose);
        _preBtn.onClick.AddListener(OnPreBtn);
        _buildingBtn.onClick.AddListener(OnBuildBtn);
        _solveBtn.onClick.AddListener(OnPreBtn);
    }

    void OnDestroy()
    {
        EventModule.Instance?.UnRegister(EventDefine.BuildingLevelup, OnBuildingLevelupEvent);
        EventModule.Instance?.UnRegister(EventDefine.BuildingLevelupFinish, OnBuildingLevelupFinishEvent);
        EventModule.Instance?.UnRegister(EventDefine.TalentResearchComplete, OnTalentResearchCompleteEvent);
        EventModule.Instance?.UnRegister(EventDefine.UpdateItem, OnUpdateItemEvent);
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
    private void RefreshBuildingContent()
    {
        if(_buildingData.Status == BuildingStatus.Unbuilt) {
            _unBuildContent.SetActive(true);
            _inBuildingContent.SetActive(false);
            RefreshPreContent();
            RefreshUnbuildingContent();
        } else if(_buildingData.Status == BuildingStatus.Progress) {
            _unBuildContent.SetActive(false);
            _inBuildingContent.SetActive(true);
            _preBtn.gameObject.SetActive(false);
            RefreshInBuildingContent();
        } else {
            Debug.Log("[BuidlingBuildView] We are in big trouble! status error = " + _buildingData.Status + ", Id = " + _buildingData.Config.id);
        }
    }
    private void RefreshUnbuildingContent()
    {
        _progressBar.fillAmount = 0;
        if(_preBuilding == null && _preTalent == null) {
            _solveBtn.gameObject.SetActive(false);
            _buildingBtn.gameObject.SetActive(true);
            _buildingTimeTxt.text = FormatUtil.FormatTimeAuto((int)_buildingData.GetProp(BuildingProperty.LvlUpDuration));
            var cost = (int)_buildingData.GetProp(BuildingProperty.LvlUpCost);
            _buildingCostTxt.text = FormatUtil.Currency(cost);
            _buildingBtn.interactable = _buildingData.CanLevelup() && (PlayerModule.Instance.Money >= cost);
        } else {
            _solveBtn.gameObject.SetActive(true);
            _buildingBtn.gameObject.SetActive(false);
        }
    }

    private void RefreshInBuildingContent()
    {
        _adTimerProgress.Init(_buildingData);
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
    private void OnBuildingLevelupEvent(Component sender, EventArgs e)
    {
        BuildingLevelupEventArgs arg = e as BuildingLevelupEventArgs;
        if(arg.buildingId == _buildingData.Config.id) {
            RefreshBuildingContent();
        }
    }
    private void OnBuildingLevelupFinishEvent(Component sender, EventArgs e)
    {
        BuildingLevelupEventArgs arg = e as BuildingLevelupEventArgs;
        if(arg.buildingId == _buildingData.Config.id) {
            Hide();
        } else if(_buildingData.Status == BuildingStatus.Unbuilt) {
            RefreshPreContent();
            RefreshUnbuildingContent();
        }
    }
    private void OnTalentResearchCompleteEvent(Component sender, EventArgs e)
    {
        if(_buildingData.Status == BuildingStatus.Unbuilt) {
            RefreshPreContent();
            RefreshUnbuildingContent();
        }
    }
    private void OnUpdateItemEvent(Component sender, EventArgs e)
    {
        UpdateItemArgs arg = e as UpdateItemArgs;
        if(arg.item == ItemType.Money && _buildingData.Status == BuildingStatus.Unbuilt) {
            RefreshUnbuildingContent();
        }
    }
}