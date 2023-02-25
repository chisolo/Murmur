using System;
using UnityEngine;

public class BureauBuildingView : PopupUIBaseCtrl
{
    public class BureauBuildingViewArgs : PopupUIArgs
    {
        public string buildingId;
    }
    public static string PrefabPath = "Assets/Res/UI/Prefab/Building/BureauBuildingView.prefab";

    [SerializeField] BureauBuildingInfoView _bureauBuildingInfoView;
    [SerializeField] BureauStaffInfoView _bureauStaffInfoView;

    private BuildingData _buildingData;

    public override void Init(PopupUIArgs arg)
    {
        EventModule.Instance.Register(EventDefine.BuildingEnhanceLevelup, OnBuildingEnhanceLevelupEvent);
        EventModule.Instance.Register(EventDefine.BuildingLevelup, OnBuildingLevelupEvent);
        EventModule.Instance.Register(EventDefine.BuildingLevelupFinish, OnBuildingLevelupFinishEvent);
        EventModule.Instance.Register(EventDefine.TalentResearchComplete, OnTalentResearchCompleteEvent);
        EventModule.Instance.Register(EventDefine.UpdateItem, OnUpdateItemEvent);

        _buildingData = BuildingModule.Instance.GetBuilding(((BureauBuildingViewArgs)arg).buildingId);
        _bureauBuildingInfoView.Init(_buildingData, this);

        _bureauStaffInfoView.Init(_buildingData);
    }
    void OnDestroy()
    {
        EventModule.Instance?.UnRegister(EventDefine.BuildingEnhanceLevelup, OnBuildingEnhanceLevelupEvent);
        EventModule.Instance?.UnRegister(EventDefine.BuildingLevelup, OnBuildingLevelupEvent);
        EventModule.Instance?.UnRegister(EventDefine.BuildingLevelupFinish, OnBuildingLevelupFinishEvent);
        EventModule.Instance?.UnRegister(EventDefine.TalentResearchComplete, OnTalentResearchCompleteEvent);
        EventModule.Instance?.UnRegister(EventDefine.UpdateItem, OnUpdateItemEvent);
    }
    private void OnBuildingEnhanceLevelupEvent(Component sender, EventArgs e)
    {
        BuildingLevelupEventArgs args = e as BuildingLevelupEventArgs;
        if(_buildingData.Config.id != args.buildingId) return;
        _bureauBuildingInfoView.OnLevelupEnhance(args.enhanceId);

        _bureauStaffInfoView.RefreshSalary();
    }
    private void OnBuildingLevelupEvent(Component sender, EventArgs e)
    {
        BuildingLevelupEventArgs args = e as BuildingLevelupEventArgs;
        if(_buildingData.Config.id != args.buildingId) return;
        _bureauBuildingInfoView.OnLevelup();
    }
    private void OnBuildingLevelupFinishEvent(Component sender, EventArgs e)
    {
        BuildingLevelupEventArgs args = e as BuildingLevelupEventArgs;
        if(_buildingData.Config.id == args.buildingId) {
            _bureauBuildingInfoView.OnLevelupFinish();
        } else if(_buildingData.Status == BuildingStatus.Normal) {
            _bureauBuildingInfoView.RefreshReadyContent();
        }
    }
    private void OnTalentResearchCompleteEvent(Component sender, EventArgs e)
    {
        if(_buildingData.Status == BuildingStatus.Normal) {
            _bureauBuildingInfoView.RefreshReadyContent();
        }
    }
    private void OnUpdateItemEvent(Component sender, EventArgs e)
    {
        UpdateItemArgs arg = e as UpdateItemArgs;
        if(arg.item == ItemType.Money) {
            _bureauBuildingInfoView.OnUpdateMoney();
        }
    }
}
