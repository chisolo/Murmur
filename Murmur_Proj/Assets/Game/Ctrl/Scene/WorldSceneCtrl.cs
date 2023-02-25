using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lemegeton;

public class WorldSceneCtrl : MonoBehaviour
{
    [SerializeField] CameraCtrl cameraCtrl;
    void Awake()
    {
        var common = ConfigModule.Instance.Common();
        cameraCtrl.SetDefaultPos(common.camera_pos);
        cameraCtrl.SetViewDistance(common.camera_min_dis, common.camera_init_dis, common.camera_max_dis);
        cameraCtrl.SetBoundary(Vector3.zero, common.camera_boundary);
        RuntimeMgr.Instance.Init();
    }

    void Start()
    {
        RuntimeMgr.Instance.LoadMgr();
    }
}
