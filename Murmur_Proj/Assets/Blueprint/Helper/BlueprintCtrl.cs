using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lemegeton;

public class BlueprintCtrl : MonoBehaviour
{
    [SerializeField] CameraCtrl cameraCtrl;
    [SerializeField] Vector3 cameraPosition;
    [SerializeField] Vector3 cameraBoundary;
    [SerializeField] float CameraMinDistance;
    [SerializeField] float CameraMaxDistance;
    [SerializeField] float CameraInitDistance;
    void Awake()
    {
        cameraCtrl.SetDefaultPos(cameraPosition);
        cameraCtrl.SetViewDistance(CameraMinDistance, CameraInitDistance, CameraMaxDistance);
        cameraCtrl.SetBoundary(Vector3.zero, cameraBoundary);
    }
}
