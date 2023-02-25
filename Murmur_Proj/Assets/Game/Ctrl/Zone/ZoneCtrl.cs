using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Lemegeton;

public class ZoneCtrl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    protected ZoneMgr _zoneMgr;
    protected ZoneData _zoneData;
    protected Transform _rootZoneMods;
    protected Transform _rootChairMods;
    protected Transform _rootHudMods;
    protected Transform _rootItemMods;
    protected Transform _rootProductMods;

    protected GameObject _buildHudObj;
    protected GameObject _levelupHudObj;
    protected GameObject _progressHudObj;
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
    protected void OnClick()
    {
        //BuildingMgr.Instance.PopupBuildingView(_buildingData);
        //_buildingData.DumpProperty();
    }
    public async void Load(ZoneData buildingData, Action callback)
    {
        _zoneMgr = ZoneMgr.Instance;
        _zoneData = buildingData;

        _rootZoneMods = new GameObject("zone").transform;
        _rootZoneMods.SetParent(transform);
        _rootChairMods = new GameObject("chair").transform;
        _rootChairMods.SetParent(transform);
        _rootHudMods = new GameObject("build_hud").transform;
        _rootHudMods.SetParent(transform);
        _rootItemMods = new GameObject("item").transform;
        _rootItemMods.SetParent(transform);
        _rootProductMods = new GameObject("product").transform;
        _rootProductMods.SetParent(transform);

        _buildHudObj = null;
        _levelupHudObj = null;
        _progressHudObj = null;
    
        _timerId = -1;
        OnClickTriggerMod += OnClick;
        InitFlow();
        await LoadTask();
        callback?.Invoke();
    }
    public async Task LoadTask()
    {
        await LoadClickMod();
        await LoadZoneMod();
        await RefreshChairMod();
        await RefreshHud();
        await RefreshChairMod();
        await RefreshProductHud();
    }
    protected async Task LoadClickMod()
    {
        await _zoneMgr.LoadModTask(ClickModPath(), transform);
    }
    protected async Task LoadZoneMod()
    {
        _zoneMgr.DestroyChildren(_rootZoneMods);
        await _zoneMgr.LoadModTask(ZoneModPath(), _rootZoneMods);
    }
    protected async Task RefreshChairMod()
    {
        var count = _rootChairMods.childCount;
        var capacity = (int)_zoneData.GetProp(ZoneProperty.WaitingCapacity);
        if(count >= capacity) return;

        for(int i = count + 1; i <= capacity; ++i) {
            await _zoneMgr.LoadModTask(ZoneChairPath(i), transform);
        }
    }
    protected async Task RefreshHud()
    {
        if(_zoneData.Status == ZoneStatus.Unbuilt && _zoneData.ZoneRequire() && _buildHudObj == null) {
            _buildHudObj = await _zoneMgr.LoadModGameObjectTask(ZoneBuildHudPath(), _rootHudMods);
        } else if(_buildHudObj != null){
            Destroy(_buildHudObj);
            _buildHudObj = null;
        }

        if(_zoneData.Status == ZoneStatus.Built && _zoneData.ZoneRequire() && _levelupHudObj == null) {
            _levelupHudObj = await _zoneMgr.LoadModGameObjectTask(ZoneLevelupHudPath(), _rootHudMods);
        } else if(_levelupHudObj != null) {
            Destroy(_levelupHudObj);
            _levelupHudObj = null;
        }

        if(_zoneData.Status == ZoneStatus.Levelup) {
            _progressHudObj = await _zoneMgr.LoadModGameObjectTask(ZoneLevelupHudPath(), _rootHudMods);
        } else if(_progressHudObj != null) {
            Destroy(_progressHudObj);
            _progressHudObj = null;
        }
    }
    private string ClickModPath()
    {
        return string.Format("{0}_{1}", _zoneData.Config.id, "click");
    }
    private string ZoneModPath()
    {
        var level = _zoneData.Level;
        if(level > 1) level = 1;
        return string.Format("{0}_{1}_{2}", _zoneData.Config.id, ZoneConst.Click, level);
    }
    public string ZoneItemModPath(string itemId, int itemLevel)
    {
        return string.Format("{0}_{1}_{2}_{3}", _zoneData.Config.id, itemId, ZoneConst.Mod, itemLevel);
    }
    public string ZoneChairPath(int index)
    {
        return string.Format("{0}_{1}_{2}", _zoneData.Config.id, ZoneConst.Chair, index);
    }
    public string ZoneBuildHudPath()
    {
        return string.Format("{0}_{1}", _zoneData.Config.id, ZoneConst.Build);
    }
    public string ZoneLevelupHudPath()
    {
        return string.Format("{0}_{1}", _zoneData.Config.id, ZoneConst.Levelup);
    }
    public async void OnZoneItemLevelup()
    {
        await RefreshChairMod();
        await RefreshHud();
        await RefreshStaffHud();
        await RefreshProductHud();
        RefreshWaitingCapacity();
    }
    public async void OnZoneLevelup()
    {
        await RefreshHud();
        await RefreshProductHud();
    }
    public async void OnZoneLevelupDone(string zoneId)
    {
        // TODO
    }
    public async void OnTalentComplete()
    {
        // TODO
    }
    public async void OnAssginStaff()
    {
        // TODO
    }
    #region 虚函数
    public virtual void InitFlow() {}
    public virtual void AddGuest(PuppetCtrl guest) {}
    public virtual bool CanAddGuest() { return false; }
    public virtual void RemoveGuest(PuppetCtrl guest) {}
    public virtual int LineupGuestCount() { return 0; }
    public virtual async Task RefreshStaffHud() { await Task.Yield(); }
    public virtual async Task RefreshProductHud() { await Task.Yield(); }
    public virtual void RefreshProduct() {}
    public virtual void RefreshWorker() {}
    public virtual void RefreshStaff() {}
    public virtual void RefreshWaitingCapacity() {}
    public virtual void SpawnInitGuest() {}
    #endregion
}