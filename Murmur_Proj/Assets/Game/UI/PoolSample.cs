using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lemegeton;

public class PoolSample : MonoBehaviour
{

    List<GameObject> _objList = new List<GameObject>();

    [SerializeField]
    private Transform _root;

    private string path = "Assets/Bundles/Prefab/PoolSamplePrefab.prefab";

    public void onClickPoolAdd()
    {
        var ps = AddressablePoolModule.Instance.Get(path);
        _objList.Add(ps);

        ps.transform.parent = _root;
    }

    public void onClickPoolReturn()
    {
        if (_objList.Count == 0) {
            return;
        }

        Debug.Log("onClickPoolReturn");
        AddressablePoolModule.Instance.Return(path, _objList[0]);

        _objList.RemoveAt(0);
    }

    public void onClickPoolLoad()
    {
        AddressablePoolModule.Instance.Prepare(path);
    }

    public void onClickPoolClear()
    {
        AddressablePoolModule.Instance.Clear(path);
    }

    public void onClickLog()
    {
        Debug.Log("123456");
    }

}


