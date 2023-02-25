using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Lemegeton;

public class BuildingCtrl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    protected BuildingData _buildingData;
    protected bool _loaded;
    protected Transform _modsRootTrans;

    protected GameObject _levelUpHub;
    protected GameObject _staffHub;
    protected BuildingProgressView _progressView;
    private long _timerId;
    public Action OnClickTriggerMod;
    public void OnPointerDown(PointerEventData eventData)
    {
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (Vector2.Distance(eventData.pressPosition, eventData.position) > 3) return;
        OnClickTriggerMod?.Invoke();
    }
    public async void Load(BuildingData buildingData, Action callback)
    {
        _loaded = false;
        _buildingData = buildingData;
        _modsRootTrans = new GameObject("mods").transform;
        _modsRootTrans.SetParent(transform);
        _timerId = -1;
        OnClickTriggerMod += OnClick;
        InitFlow();
        await LoadTask();
        _loaded = true;
        callback?.Invoke();
    }
    public async Task LoadTask()
    {
        await LoadClickMod();
        await LoadBuildingMod();
        await LoadExtraHub();
        
        await RefreshBuldingProgressHub();
        await RefreshLevelupHub();
        await RefreshStaffHub();
    }
    protected async Task LoadModTask(ModConfig modConfig, Transform parent)
    {
        if(modConfig == null) return;
        GameObject mod = await PrefabLoader.InstantiateAsyncTask(modConfig.asset, modConfig.position, modConfig.rotation, parent);
        mod.name = modConfig.id;
        mod.transform.localScale = modConfig.scale;
    }
    protected async Task<GameObject> LoadModGameObjectTask(ModConfig modConfig, Transform parent)
    {
        if(modConfig == null) return null;
        GameObject mod = await PrefabLoader.InstantiateAsyncTask(modConfig.asset, modConfig.position, modConfig.rotation, parent);
        mod.name = modConfig.id;
        mod.transform.localScale = modConfig.scale;
        return mod;
    }
    protected async Task<T> LoadModComponentTask<T>(ModConfig modConfig, Transform parent)
    {
        if(modConfig == null) return default(T);
        GameObject mod = await PrefabLoader.InstantiateAsyncTask(modConfig.asset, modConfig.position, modConfig.rotation, parent);
        mod.name = modConfig.id;
        mod.transform.localScale = modConfig.scale;
        return mod.GetComponent<T>();
    }
    protected void DestroyChildren(Transform parent)
    {
        var childCount = parent.childCount;
        for(int i = 0; i < childCount; ++i) {
            DestroyImmediate(parent.transform.GetChild(0).gameObject);
        }
    }
    protected async Task LoadClickMod()
    {
        var modConfig = ConfigModule.Instance.GetMod(_buildingData.Config.click_mod);
        await LoadModTask(modConfig, transform);
    }
    protected async Task LoadBuildingMod()
    {
        DestroyChildren(_modsRootTrans);
        if(_buildingData.Config.mods.Count == 0) return;
        var modConfig = ConfigModule.Instance.GetMod(_buildingData.Config.mods[_buildingData.GetLevel()]);
        await LoadModTask(modConfig, _modsRootTrans);
    }
    protected async Task LoadWaitingMod(int index)
    {
        if(string.IsNullOrEmpty(_buildingData.Config.waiting_mod)) return;
        var modConfig = ConfigModule.Instance.GetMod(_buildingData.Config.waiting_mod);
        await LoadModTask(modConfig, transform);
    }
    protected async Task RefreshBuldingProgressHub()
    {
        if(_buildingData.Status == BuildingStatus.Progress) {
            var remain = _buildingData.GetWorkTeamRemainTime();
            var total = _buildingData.GetWorkTeamDuration();
            if(_timerId < 0) _timerId = TimerModule.Instance.CreateFrameTimer(remain, null, true, RefreshLevelup);
            else TimerModule.Instance.UpdateTimer(_timerId, remain);

            if(_progressView == null) {
                var modConfig = ConfigModule.Instance.GetMod(GetBuildingProgressPath());
                _progressView = await LoadModComponentTask<BuildingProgressView>(modConfig, transform);
                _progressView.Show(remain, total, RuntimeMgr.Instance.GetWorldCamera());
            }
        } else {
            _progressView?.Hide();
            _progressView = null;
        }
    }
    protected async Task RefreshLevelupHub()
    {
        if(_buildingData.Status == BuildingStatus.Unbuilt && _buildingData.PreBuildingFit()) {
            if(_levelUpHub == null) {
                var modConfig = ConfigModule.Instance.GetMod(GetBuildHubPath());
                _levelUpHub = await LoadModGameObjectTask(modConfig, transform);
            }
        } else if(_buildingData.Status == BuildingStatus.Normal && _buildingData.PreBuildingFit()) {
            if(_levelUpHub == null) {
                var modConfig = ConfigModule.Instance.GetMod(GetLevelupHubPath());
                _levelUpHub = await LoadModGameObjectTask(modConfig, transform);
            }
        } else {
            if(_levelUpHub != null) {
                Destroy(_levelUpHub);
                _levelUpHub = null;
            }
        }
    }
    protected async Task RefreshStaffHub()
    {
        if(_buildingData.Config.type == BuidlingType.Bureau && _buildingData.GetLevel() > 0 && string.IsNullOrEmpty(_buildingData.GetStaff())) {
            if(_staffHub == null) {
                var modConfig = ConfigModule.Instance.GetMod(GetStaffHubPath());
                _staffHub = await LoadModGameObjectTask(modConfig, transform);
            }
        } else {
            if(_staffHub != null) {
                Destroy(_staffHub);
                _staffHub = null;
            }
        }
    }
    public async void OnEnhanceLevelup()
    {
        RefreshMoveSpeed();
        RefreshCapacity();

        await RefreshLevelupHub();
    }
    private void RefreshLevelup(float left)
    {
        _progressView?.Refresh(left);
    }
    public async void OnLevelup()
    {
        await RefreshBuldingProgressHub();
        await RefreshLevelupHub();
    }
    public async void OnReduceTime()
    {
        await RefreshBuldingProgressHub();
    }
    public async void OnFinishLevelup()
    {
        if(_timerId > 0) {
            TimerModule.Instance.CancelTimer(_timerId);
            _timerId = -1;
        }
        RefreshCapacity();
        RefreshWorker();
        await LoadBuildingMod();
        await RefreshBuldingProgressHub();
        await RefreshLevelupHub();
        await RefreshStaffHub();
    }
    public async void OnAssignStaff()
    {
        await RefreshStaffHub();

        RefreshStaff();
    }
    public async void OnPreLevelup()
    {
        await RefreshLevelupHub();
    }
    protected void OnClick()
    {
        BuildingMgr.Instance.PopupBuildingView(_buildingData);
        //_buildingData.DumpProperty();
    }
    private string GetBuildingProgressPath()
    {
        return string.Format("{0}_{1}", _buildingData.Config.id, "progress");
    }
    private string GetLevelupHubPath()
    {
        return string.Format("{0}_{1}", _buildingData.Config.id, "hub_levelup");
    }
    private string GetBuildHubPath()
    {
        return string.Format("{0}_{1}", _buildingData.Config.id, "hub_build");
    }
    private string GetStaffHubPath()
    {
        return string.Format("{0}_{1}", _buildingData.Config.id, "hub_staff");
    }
    #region 虚函数
    public virtual void InitFlow() {}
    public virtual async Task LoadExtraHub() { await Task.Yield();}
    public virtual void AddGuest(PuppetCtrl guest) {}
    public virtual bool CanAddGuest() { return false; }
    public virtual void RemoveGuest(PuppetCtrl guest) {}
    public virtual int LineupGuestCount() { return 0; }
    public virtual void RefreshIngredient() {}
    public virtual void RefreshWorker() {}
    public virtual void RefreshStaff() {}
    public virtual void RefreshMoveSpeed() {}
    public virtual void RefreshCapacity() {}
    public virtual void SpawnInitGuest() {}
    #endregion
}
