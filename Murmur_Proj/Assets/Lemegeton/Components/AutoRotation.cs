using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotation : MonoBehaviour
{
    [SerializeField] Transform parentTrans;

    void Update()
    {
        transform.localRotation = Quaternion.Euler(0, -parentTrans.localRotation.eulerAngles.y, 0);
    }
}
