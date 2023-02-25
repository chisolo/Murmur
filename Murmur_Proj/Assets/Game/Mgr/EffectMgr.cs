using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lemegeton;
using System.Threading.Tasks;

public class EffectMgr : GameMgr<EffectMgr>
{
    public static string SmallMoneyEffect = "Assets/Res/Fx/Effect/cash_small_effect.prefab";
    public static string BigMoneyEffect = "Assets/Res/Fx/Effect/cash_big_effect.prefab";
    public static string BigCoinEffect = "Assets/Res/Fx/Effect/coin_big_effect.prefab";
    
    [SerializeField] Transform _effectRoot;
    [SerializeField] Transform _screenRoot;

    void Awake()
    {
        if (_effectRoot == null) {
            Debug.LogError("not setup");
        }
        EventModule.Instance.Register(EventDefine.ItemEffect, OnItemEffectEvent);
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        EventModule.Instance?.UnRegister(EventDefine.ItemEffect, OnItemEffectEvent);
    }
    public async void PlayWorld(string address, Vector3 position)
    {
        var go = await AddressablePoolModule.Instance.LoadAndGetAsyncTask(address);
        if (go != null) {
            go.transform.parent = _effectRoot;
            // 更新世界坐标
            go.transform.position = position;
            go.transform.rotation = Quaternion.identity;

            var effect = go.GetComponent<EffectAsset>();
            effect.onStop = () => {
                AddressablePoolModule.Instance.Return(address, go);
            };
        }
    }

    public async void PlayScreen(string address, Vector3 position)
    {
        var go = await AddressablePoolModule.Instance.LoadAndGetAsyncTask(address);
        if (go != null) {
            go.transform.parent = _screenRoot;
            // 更新本地坐标
            go.transform.localPosition = position;

            var effect = go.GetComponent<EffectAsset>();
            effect.onStop = () => {
                AddressablePoolModule.Instance.Return(address, go);
            };
        }
    }
    private void OnItemEffectEvent(Component sender, EventArgs e)
    {
        ItemEffectEventArgs arg = e as ItemEffectEventArgs;
        if(arg.effect == ItemEffectType.SmallMoney) {
            PlayWorld(SmallMoneyEffect, arg.pos);
        } else if(arg.effect == ItemEffectType.BigMoney) {
            PlayScreen(BigMoneyEffect, arg.pos);
        } else if(arg.effect == ItemEffectType.BigCoin) {
            PlayScreen(BigCoinEffect, arg.pos);
        }
    }
}