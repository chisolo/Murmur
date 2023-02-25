using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GMBtn : MonoBehaviour
{
    public void OnGMBtn()
    {
        UIMgr.Instance.OpenUIByClick(GMView.PrefabPath, null, false, false);
    }
}
