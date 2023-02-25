using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lemegeton;

public class RuntimeMgr : GameMgr<RuntimeMgr>
{
    [SerializeField] Camera _worldCamera;
    [SerializeField] WorldSceneCtrl _worldScene;
    [SerializeField] CameraCtrl _worldCameraCtrl;


    public bool CameraDebug {get; set;}
    public bool UIDebug {get; set;}
    public void Init()
    {
        CameraDebug = false;
        UIDebug = false;
    }
    private void ScheduleNotification()
    {
        var notifications = ConfigModule.Instance.Notifications();
        if(notifications == null) return;
        foreach(var notification in notifications) {
            
        }
    }
    public Camera GetWorldCamera()
    {
        return _worldCamera;
    }

    public CameraCtrl GetWorldCameraCtrl()
    {
        return _worldCameraCtrl;
    }

    public void SetCameraDebug()
    {
        var common = ConfigModule.Instance.Common();
        if(CameraDebug) {
            _worldCameraCtrl.SetViewDistance(common.camera_min_dis, common.camera_init_dis, common.camera_max_dis);
            CameraDebug = false;
        } else {
            _worldCameraCtrl.SetViewDistance(3f, common.camera_init_dis, 35f);
            CameraDebug = true;
        }
    }
    public void SetUIDebug()
    {
        if(UIDebug) {
            UIMgr.Instance.ShowMainParts();
            UIDebug = false;
        } else {
            UIMgr.Instance.HideMainParts();
            UIDebug = true;
        }
    }
    public async void LoadMgr()
    {
        // 注意，Load需有序
        using (LoadingProgressEventArgs args = LoadingProgressEventArgs.Get()) {
            args.progress = 0.4f;
            EventModule.Instance.FireEvent(EventDefine.LoadingProgress, args);
        }
        await PuppetMgr.Instance.Load();
        using (LoadingProgressEventArgs args = LoadingProgressEventArgs.Get()) {
            args.progress = 0.4f;
            EventModule.Instance.FireEvent(EventDefine.LoadingProgress, args);
        }
        await BuildingMgr.Instance.Load();
        using (LoadingProgressEventArgs args = LoadingProgressEventArgs.Get()) {
            args.progress = 0.1f;
            EventModule.Instance.FireEvent(EventDefine.LoadingProgress, args);
        }
    }
}
