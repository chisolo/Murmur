using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Lemegeton;

public class ZoneMgr : GameMgr<ZoneMgr>
{
    public async Task LoadModTask(ModConfig modConfig, Transform parent)
    {
        if(modConfig == null) return;
        GameObject mod = await PrefabLoader.InstantiateAsyncTask(modConfig.asset, modConfig.position, modConfig.rotation, parent);
        mod.name = modConfig.id;
        mod.transform.localScale = modConfig.scale;
    }
    public async Task LoadModTask(string modPath, Transform parent)
    {
        var modConfig = ConfigModule.Instance.GetMod(modPath);
        if(modConfig == null) return;
        GameObject mod = await PrefabLoader.InstantiateAsyncTask(modConfig.asset, modConfig.position, modConfig.rotation, parent);
        mod.name = modConfig.id;
        mod.transform.localScale = modConfig.scale;
    }
    public async Task<GameObject> LoadModGameObjectTask(ModConfig modConfig, Transform parent)
    {
        if(modConfig == null) return null;
        GameObject mod = await PrefabLoader.InstantiateAsyncTask(modConfig.asset, modConfig.position, modConfig.rotation, parent);
        mod.name = modConfig.id;
        mod.transform.localScale = modConfig.scale;
        return mod;
    }
    public async Task<GameObject> LoadModGameObjectTask(string modPath, Transform parent)
    {
        var modConfig = ConfigModule.Instance.GetMod(modPath);
        if(modConfig == null) return null;
        GameObject mod = await PrefabLoader.InstantiateAsyncTask(modConfig.asset, modConfig.position, modConfig.rotation, parent);
        mod.name = modConfig.id;
        mod.transform.localScale = modConfig.scale;
        return mod;
    }
    public async Task<T> LoadModComponentTask<T>(ModConfig modConfig, Transform parent)
    {
        if(modConfig == null) return default(T);
        GameObject mod = await PrefabLoader.InstantiateAsyncTask(modConfig.asset, modConfig.position, modConfig.rotation, parent);
        mod.name = modConfig.id;
        mod.transform.localScale = modConfig.scale;
        return mod.GetComponent<T>();
    }
    public async Task<T> LoadModComponentTask<T>(string modPath, Transform parent)
    {
        var modConfig = ConfigModule.Instance.GetMod(modPath);
        if(modConfig == null) return default(T);
        GameObject mod = await PrefabLoader.InstantiateAsyncTask(modConfig.asset, modConfig.position, modConfig.rotation, parent);
        mod.name = modConfig.id;
        mod.transform.localScale = modConfig.scale;
        return mod.GetComponent<T>();
    }
    public void DestroyChildren(Transform parent)
    {
        var childCount = parent.childCount;
        for(int i = 0; i < childCount; ++i) {
            DestroyImmediate(parent.transform.GetChild(0).gameObject);
        }
    }
}
