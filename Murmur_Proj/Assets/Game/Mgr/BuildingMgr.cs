using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Lemegeton;

public class BuildingMgr : GameMgr<BuildingMgr>
{
    private Dictionary<string, BuildingCtrl> _buildings;
    private Dictionary<string, List<BuildingCtrl>> _typeBuildings;
    private BuildingCtrl _receptionDivision;
    private bool _loaded;
    private bool _review;
    public void Start()
    {
        EventModule.Instance.Register(EventDefine.BuildingEnhanceLevelup, OnBuildingEnhanceLevelupEvent);
        EventModule.Instance.Register(EventDefine.BuildingLevelup, OnBuildingLevelupEvent);
        EventModule.Instance.Register(EventDefine.BuildingLevelupFinish, OnBuildingLevelupFinishEvent);
        EventModule.Instance.Register(EventDefine.AssignBuildingStaff, OnAssignBuildingStaffEvent);
        EventModule.Instance.Register(EventDefine.UpdateIngredient, OnUpdateIngredientEvent);
        EventModule.Instance.Register(EventDefine.ReduceBuildingTime, OnReduceBuildingTimeEvent);
        EventModule.Instance.Register(EventDefine.TutorialStart, OnTutorialStartEvent);
        EventModule.Instance.Register(EventDefine.RequestReview, OnRequestReviewEvent);
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        EventModule.Instance?.UnRegister(EventDefine.BuildingEnhanceLevelup, OnBuildingEnhanceLevelupEvent);
        EventModule.Instance?.UnRegister(EventDefine.BuildingLevelup, OnBuildingLevelupEvent);
        EventModule.Instance?.UnRegister(EventDefine.BuildingLevelupFinish, OnBuildingLevelupFinishEvent);
        EventModule.Instance?.UnRegister(EventDefine.AssignBuildingStaff, OnAssignBuildingStaffEvent);
        EventModule.Instance?.UnRegister(EventDefine.UpdateIngredient, OnUpdateIngredientEvent);
        EventModule.Instance?.UnRegister(EventDefine.ReduceBuildingTime, OnReduceBuildingTimeEvent);
        EventModule.Instance?.UnRegister(EventDefine.TutorialStart, OnTutorialStartEvent);
        EventModule.Instance?.UnRegister(EventDefine.RequestReview, OnRequestReviewEvent);
    }
    public async Task Load()
    {
        _review = false;
        LoadBuildings();
        while(!_loaded) {
            await Task.Yield();
        }
        SpawnInitGuest();
        StartCoroutine(SalaryCoroutine());
    }
    private async void LoadBuildings()
    {
        var buildingDatas = BuildingModule.Instance.GetBuildings();
        _buildings = new Dictionary<string, BuildingCtrl>();
        _typeBuildings = new Dictionary<string, List<BuildingCtrl>>();

        int loaded = 0;
        int total = buildingDatas.Count;
        foreach(var buildingData in buildingDatas.Values) {
            BuildingCtrl ctrl = InitBuildingCtrl(buildingData.Config.id, buildingData.Config.service);

            if(ctrl == null) {
                Debug.LogWarning("[BuildingMgr] LoadBuildings ctrl is null! id = " + buildingData.Config.id);
                continue;
            }

            ctrl.Load(buildingData, () => {
                _buildings.Add(buildingData.Config.id, ctrl);
                if(!_typeBuildings.ContainsKey(buildingData.Config.service)) _typeBuildings.Add(buildingData.Config.service, new List<BuildingCtrl>());
                _typeBuildings[buildingData.Config.service].Add(ctrl);
                ++loaded;
            });
        }
        while(loaded < total) {
            await Task.Yield();
        }

        AudioModule.Instance.PlayBgm(GameUtil.ResBgm);
        _loaded = true;
    }

    private BuildingCtrl InitBuildingCtrl(string id, string service)
    {
        var building = new GameObject(id);
        building.transform.SetParent(transform);
        BuildingCtrl ret = null;
        if( service == ServiceType.ReceptionDivision) {
            ret = building.AddComponent<ReceptionDivisionCtrl>();
            _receptionDivision = ret;
        } else if(service == ServiceType.ReceptionCell) {
            ret = building.AddComponent<ReceptionCellCtrl>();
        } else if(service == ServiceType.HotdogBureau) {
            ret = building.AddComponent<BureauCtrl>();
        } else if(service == ServiceType.HambergBureau) {
            ret = building.AddComponent<BureauCtrl>();
        } else if(service == ServiceType.PizzaBureau) {
            ret = building.AddComponent<BureauCtrl>();
        } else if(service == ServiceType.SausageStorageDivision) {
            ret = building.AddComponent<StorageDivisionCtrl>();
        } else if(service == ServiceType.CheeseStorageDivision) {
            ret = building.AddComponent<StorageDivisionCtrl>();
        } else if(service == ServiceType.FlourStorageDivision) {
            ret = building.AddComponent<StorageDivisionCtrl>();
        } else if(service == ServiceType.SausageStorageCell) {
            ret = building.AddComponent<StorageCellCtrl>();
        } else if(service == ServiceType.CheeseStorageCell) {
            ret = building.AddComponent<StorageCellCtrl>();
        } else if(service == ServiceType.FlourStorageCell) {
            ret = building.AddComponent<StorageCellCtrl>();
        } else if(service == ServiceType.DeliverDivision) {
            ret = building.AddComponent<DeliverDivisionCtrl>();
        } else if(service == ServiceType.DeliverCell) {
            ret = building.AddComponent<DeliverCellCtrl>();
        } else if(service == ServiceType.TreasurySupply) {
            ret = building.AddComponent<TreasurySupplyCtrl>();
        }
        return ret;
    }
    IEnumerator SalaryCoroutine()
    {
        yield return new WaitForSeconds(GameUtil.DefaultTimeInterval);
        if(TutorialModule.Instance.IsComplete(GameUtil.TriggerTutorialId)) {
            PlayerModule.Instance.UseMoney((int)BuildingModule.Instance.GetTotalProp(BuildingProperty.Salary) + (int)BuildingModule.Instance.GetTotalProp(BuildingProperty.Revenue), true);
        }
    }
    #region 获取相关接口
    public BuildingCtrl GetBuildingCtrl(string buildingId)
    {
        return _buildings.TryGetValue(buildingId, out var value) ? value : null;
    }
    public BuildingCtrl GetBuildingCtrl(string buildingId, int index)
    {
        if(_typeBuildings.TryGetValue(buildingId, out var value)) {
            if(index < 0 && index >= value.Count) return value[index];
        }
        return null;
    }
    public List<BuildingCtrl> GetBuildingCtrls(string service)
    {
        return _typeBuildings.TryGetValue(service, out var value) ? value : null;
    }
    #endregion

    #region  消息处理
    private void OnBuildingEnhanceLevelupEvent(Component sender, EventArgs e)
    {
        BuildingLevelupEventArgs args = e as BuildingLevelupEventArgs;
        if(_buildings.TryGetValue(args.buildingId, out var buildingCtrl)) {
            buildingCtrl.OnEnhanceLevelup();
        }
        var parentId = BuildingModule.Instance.GetParentId(args.buildingId);
        if(!string.IsNullOrEmpty(parentId)) {
            if(_buildings.TryGetValue(parentId, out var parentCtrl)) {
                parentCtrl.RefreshCapacity();
            }
        }
    }
    private void OnBuildingLevelupEvent(Component sender, EventArgs e)
    {
        BuildingLevelupEventArgs args = e as BuildingLevelupEventArgs;
        if(_buildings.TryGetValue(args.buildingId, out var buildingCtrl)) {
            buildingCtrl.OnLevelup();
        }
    }
    private void OnBuildingLevelupFinishEvent(Component sender, EventArgs e)
    {
        BuildingLevelupEventArgs args = e as BuildingLevelupEventArgs;
        var buildingId = args.buildingId;
        var parentId = BuildingModule.Instance.GetParentId(args.buildingId);

        foreach(var building in _buildings) {
            var buildingCtrl = building.Value;
            if(building.Key == buildingId) {
                buildingCtrl.OnFinishLevelup();
            } else {
                if(building.Key == parentId) buildingCtrl.RefreshCapacity();
                buildingCtrl.OnPreLevelup();
            }
        }
    }
    private void OnAssignBuildingStaffEvent(Component sender, EventArgs e)
    {
        BuildingStaffEventArgs args = e as BuildingStaffEventArgs;
        if(_buildings.TryGetValue(args.buildingId, out var buildingCtrl)) {
            buildingCtrl.OnAssignStaff();
        }
    }
    private void OnUpdateIngredientEvent(Component sender, EventArgs e)
    {
        UpdateIngredientArgs args = e as UpdateIngredientArgs;
        if(_buildings.TryGetValue(args.buildingId, out var buildingCtrl)) {
            buildingCtrl.RefreshIngredient();
        }
    }
    private void OnReduceBuildingTimeEvent(Component sender, EventArgs e)
    {
        ReduceBuildingTimeEventArgs args = e as ReduceBuildingTimeEventArgs;
        if(_buildings.TryGetValue(args.zoneId, out var buildingCtrl)) {
            buildingCtrl.OnReduceTime();
        }
    }
    private void OnTutorialStartEvent(Component sender, EventArgs e)
    {
        TutorialStartEventArgs args = e as TutorialStartEventArgs;
        if(UIMgr.Instance.UICount > 0) {
            StartCoroutine(RetryOpenTutorialView(args.tutorialData));
        } else {
            var arg = new TutorialView.Args();
            arg.tutorialData = args.tutorialData;
            UIMgr.Instance.OpenPopUpPanel(TutorialView.PrefabPath, arg, null, TutorialModule.Instance.FinishTutorial);
        }
    }
    private IEnumerator RetryOpenTutorialView(TutorialData tutorialData)
    {
        while(true) {
            yield return new WaitForSeconds(1f);
            if(UIMgr.Instance.UICount <= 0) {
                var arg = new TutorialView.Args();
                arg.tutorialData = tutorialData;
                UIMgr.Instance.OpenPopUpPanel(TutorialView.PrefabPath, arg, null, TutorialModule.Instance.FinishTutorial);
                yield break;
            }
        }
    }

    private void OnRequestReviewEvent(Component sender, EventArgs e)
    {
        if(UIMgr.Instance.UICount > 0) {
            StartCoroutine(RequestReviewCoroutine());
        } else {
            var common = ConfigModule.Instance.Common();
            UIMgr.Instance.OpenConfirmOkCancel("RATE_TITLE".Locale(), "RATE_BODY".Locale(), "RATE_OK".Locale(), "RATE_CANCEL".Locale(), 
                () => {
                    MaxSDK.Instance.RequestReview((int ret, string msg) => 
                    {
                        if(ret == 1) PlayerModule.Instance.OnReview();
                    });
            });
        }
    }

    private IEnumerator RequestReviewCoroutine()
    {
        while(true) {
            yield return new WaitForSeconds(1f);
            if(UIMgr.Instance.UICount <= 0) {
                var common = ConfigModule.Instance.Common();
                UIMgr.Instance.OpenConfirmOkCancel("RATE_TITLE".Locale(), "RATE_BODY".Locale(), "RATE_OK".Locale(), "RATE_CANCEL".Locale(), 
                    () => {
                        MaxSDK.Instance.RequestReview((int ret, string msg) => 
                        {
                            if(ret == 1) PlayerModule.Instance.OnReview();
                        });
                });
                yield break;
            }
        }
    }
    #endregion

    private void SpawnInitGuest()
    {
        if(!TutorialModule.Instance.IsComplete(GameUtil.TriggerTutorialId)) return;
        foreach(var building in _buildings.Values) {
            building.SpawnInitGuest();
        }
    }
    public void OnGuestSpawn(PuppetCtrl guest)
    {
        _receptionDivision.AddGuest(guest);
    }

    public void AssignGuest(PuppetCtrl guest)
    {
        var service = guest.Target;
        var building = GetValidBuilding(guest.Target);
        if(building == null) {
            guest.EmojiView.Show(EmojiType.Angry);
            PuppetMgr.Instance.PuppetExit(guest);
        } else {
            building.AddGuest(guest);
        }
    }

    public BuildingCtrl GetValidBuilding(string service)
    {
        var buildings = GetBuildingCtrls(service);
        BuildingCtrl retBuilding = null;
        int lineCount = int.MaxValue;
        foreach(var building in buildings) {
            if(!building.CanAddGuest()) continue;
            var guestCount = building.LineupGuestCount();
            if(guestCount < lineCount) {
                lineCount = guestCount;
                retBuilding = building;
            }
        }
        return retBuilding;
    }
    public void PopupBuildingView(string buildingId, bool moveCamera = false)
    {
        PopupBuildingView(BuildingModule.Instance.GetBuilding(buildingId), moveCamera);
    }

    public void PopupBuildingView(BuildingData buildingData, bool moveCamera = false)
    {
        var buildingLevel = buildingData.GetLevel();
        if(buildingLevel <= 0) {
            var arg = new BuildingBuildView.Args();
            arg.buildingId = buildingData.Config.id;
            UIMgr.Instance.OpenUIByClick(BuildingBuildView.PrefabPath, arg, false, true);
        } else {
            var buildingtype = buildingData.Config.type;
            if(buildingtype == BuidlingType.Bureau) {
                var arg = new BureauBuildingView.BureauBuildingViewArgs();
                arg.buildingId = buildingData.Config.id;
                UIMgr.Instance.OpenUIByClick(BureauBuildingView.PrefabPath, arg, false, true);
            } else if(buildingtype == BuidlingType.Division) {
                var arg = new DivisionBuildingView.DivisionBuildingViewArgs();
                arg.buildingId = buildingData.Config.id;
                UIMgr.Instance.OpenUIByClick(DivisionBuildingView.PrefabPath, arg, false, true);
            } else if(buildingtype == BuidlingType.Cell) {
                var arg = new CellBuildingView.CellBuildingViewArgs();
                arg.buildingId = buildingData.Config.id;
                UIMgr.Instance.OpenUIByClick(CellBuildingView.PrefabPath, arg, false, true);
            } else if(buildingtype == BuidlingType.Supply) {
                var arg = new SupplyBuildingView.Args();
                arg.buildingId = buildingData.Config.id;
                UIMgr.Instance.OpenUIByClick(SupplyBuildingView.PrefabPath, arg, false, true);
            }
        }

        if(moveCamera) {
            if(buildingData.Config.zoom_dis <= 0) return;
            var cameraCtrl = RuntimeMgr.Instance.GetWorldCameraCtrl();
            cameraCtrl.MoveCameraTo(buildingData.Config.zoom_pos, buildingData.Config.zoom_dis);
        }
    }
}