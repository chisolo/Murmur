#if UNITY_ANDROID
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using Max.ThirdParty;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Threading;

public class MaxSDKAndroid : MaxSDKBase
{
    private static readonly AndroidJavaClass UnityPluginClass = new AndroidJavaClass("com.flowhw.sdk.Flow998_UnityPlugin");

    private class AndroidCallback<T> : AndroidJavaProxy
    {
        private readonly Action<int, string, T> action;
        private readonly Func<string, T> parser;

        public AndroidCallback(Action<int, string, T> action, Func<string, T> parser) : base("com.flowhw.sdk.Flow998_UnityCallback")
        {
            this.action = action;
            this.parser = parser;
        }

        public void onFinish(int ret, string msg, string callbackData)
        {
            if (action != null)
            {
                action(ret, msg, parser(callbackData));
            }
        }
    }

    private class AndroidCallbackNoData : AndroidJavaProxy
    {
        private readonly Action<int, string> action;

        public AndroidCallbackNoData(Action<int, string> action) : base("com.flowhw.sdk.Flow998_UnityCallback")
        {
            this.action = action;
        }

        public void onFinish(int ret, string msg, string callbackData)
        {
            if (action != null)
            {
                action(ret, msg);
            }
        }
    }

    private void CallStatic(string method, params object[] args)
    {
        if (MaxSDKBehaviour.IsMainThread)
        {
            UnityPluginClass.CallStatic(method, args);
        }
        else
        {
            MaxSDKBehaviour.QueueOnMainThread(() =>
            {
                UnityPluginClass.CallStatic(method, args);
            });
        }
    }

    public override void Init(MaxInitOptions option, Action<int, string, MaxInitData> action)
    {
        var cb = new AndroidCallback<MaxInitData>(action, MaxInitData.Parse);
        CallStatic("init", option.ToString(), cb);
    }

    public override void AbtestConfig(Action<int, string, Dictionary<string, string>> action)
    {
        var cb = new AndroidCallback<Dictionary<string, string>>(action, ParseDictionary);
        CallStatic("abtestConfig", cb);
    }

    public override void PushEvent(string _event, Dictionary<string, string> data)
    {
        CallStatic("pushEvent", _event, Max.ThirdParty.Json.Serialize(data));
    }

    public override void Login(Action<int, string, MaxLoginData> loginAction, Action<int, string, MaxChargeData> chargeAction)
    {
        var cb1 = new AndroidCallback<MaxLoginData>(loginAction, MaxLoginData.Parse);
        if (chargeAction == null)
        {
            CallStatic("login", cb1, null);
        }
        else
        {
            var cb2 = new AndroidCallback<MaxChargeData>(chargeAction, MaxChargeData.Parse);
            CallStatic("login", cb1, cb2);
        }
    }

    public override void RewardAdPreload(string unionId, Action<int, string, string> action)
    {
        var cb = new AndroidCallback<string>(action, ParseString);
        CallStatic("rewardAdPreLoad", "", cb);
    }

    public override void RewardAdShow(string unionId, string pname, Action<int, string, string> action)
    {
        //var cb = new AndroidCallback<string>(action, ParseString);
        //CallStatic("rewardAdShow", "", pname, cb);
        MaxSDKBehaviour.Current.ShowAd(1, "", pname, action);
    }

    public override void FullVideoAdPreload(string unionId)
    {
        CallStatic("fullVideoAdPreload", "");
    }

    public override void FullVideoAdShow(string unionId, string pname, bool skipLoading, Action<int, string, string> action)
    {
        //var cb = new AndroidCallback<string>(action, ParseString);
        //CallStatic("fullVideoAdShow", "", pname, cb);
        MaxSDKBehaviour.Current.ShowAd(2, "", pname, action);
    }

    private volatile int bannerAdFitHeightPx = -1;

    public override int BannerAdFitHeightPx()
    {
        if (bannerAdFitHeightPx == -1)
        {
            bool isMainThread = MaxSDKBehaviour.IsMainThread;
            if (!isMainThread)
            {
                AndroidJNI.AttachCurrentThread();
            }
            bannerAdFitHeightPx = UnityPluginClass.CallStatic<int>("bannerAdFitHeightPx");
            if (!isMainThread)
            {
                AndroidJNI.DetachCurrentThread();
            }
        }
        return bannerAdFitHeightPx;
    }

    public override void BannerAdInit(List<MaxBannerOptions> options)
    {
        var list = new List<Dictionary<string, object>>();
        foreach (var option in options)
        {
            list.Add(option.ToDictionary());
        }
        CallStatic("bannerAdInit", Max.ThirdParty.Json.Serialize(list));
    }

    public override void BannerAdShow(List<string> pnames)
    {
        CallStatic("bannerAdShow", Max.ThirdParty.Json.Serialize(pnames));
    }

    public override void ChargeSetCallback(Action<int, string, MaxChargeData> chargeAction)
    {
        var cb2 = new AndroidCallback<MaxChargeData>(chargeAction, MaxChargeData.Parse);
        CallStatic("chargeSetCallback", cb2);
    }

    public override void Charge(string productId, string consumeExt = null)
    {
        CallStatic("charge", productId, consumeExt);
    }

    public override void ChargeRecovery()
    {
        CallStatic("chargeRecovery");
    }

    public override void ChargeProducts(List<string> productIds, Action<int, string, List<MaxChargeProductData>> action)
    {
        var cb = new AndroidCallback<List<MaxChargeProductData>>(action, MaxChargeProductData.Parse);
        CallStatic("chargeProducts", Max.ThirdParty.Json.Serialize(productIds), cb);
    }

    public override void ChargeFinish(string orderSn)
    {
        CallStatic("chargeFinish", orderSn);
    }

    public override void RequestReview(Action<int, string> action)
    {
        var cb = new AndroidCallbackNoData(action);
        CallStatic("requestReview", cb);
    }

    public override void SocialInvite(string type, string name, string ext, Action<int, string> action)
    {
        var cb = new AndroidCallbackNoData(action);
        CallStatic("socialInvite", type, name, ext, cb);
    }

    public override void SocialInviteCount(string name, Action<int, string, int> action)
    {
        var cb = new AndroidCallback<int>(action, ParseInt);
        CallStatic("socialInviteCount", name, cb);
    }

    public override void SocialFrom(Action<int, string, MaxSocialInviteFromData> action)
    {
        var cb = new AndroidCallback<MaxSocialInviteFromData>(action, MaxSocialInviteFromData.Parse);
        CallStatic("socialFrom", cb);
    }

    public override void SocialInviteReward()
    {
        CallStatic("socialInviteReward");
    }

    public override void CustomLoginToken(Action<int, string, MaxLoginData> loginAction, Action<int, string, MaxChargeData> chargeAction)
    {
        var cb1 = new AndroidCallback<MaxLoginData>(loginAction, MaxLoginData.Parse);
        if (chargeAction == null)
        {
            CallStatic("cutomLoginToken", cb1, null);
        }
        else
        {
            var cb2 = new AndroidCallback<MaxChargeData>(chargeAction, MaxChargeData.Parse);
            CallStatic("customLoginToken", cb1, cb2);
        }
    }

    public override void CustomLogin(string type, Action<int, string, MaxLoginData> loginAction, Action<int, string, MaxChargeData> chargeAction)
    {
        var cb1 = new AndroidCallback<MaxLoginData>(loginAction, MaxLoginData.Parse);
        if (chargeAction == null)
        {
            CallStatic("cutomLogin", type, cb1, null);
        }
        else
        {
            var cb2 = new AndroidCallback<MaxChargeData>(chargeAction, MaxChargeData.Parse);
            CallStatic("customLogin", type, cb1, cb2);
        }
    }

    public override void Logout(Action<int, string> action)
    {
        var cb = new AndroidCallbackNoData(action);
        CallStatic("logout", cb);
    }

    public override void Link(string type, bool shouldSwitch, Action<int, string, MaxLoginData> action)
    {
        var cb = new AndroidCallback<MaxLoginData>(action, MaxLoginData.Parse);
        CallStatic("link", type, shouldSwitch, action);
    }

    public override List<string> getLinkedList()
    {
        List<string> list = null;
        string listStr = UnityPluginClass.CallStatic<string>("getLinkedList");
        if (listStr == null || listStr.Length == 0)
        {
            return new List<string>();
        }
        try
        {
            list = Json.Deserialize(listStr) as List<string>;
        } catch(Exception ex)
        {
            UnityEngine.Debug.Log("getLinkedList exception - " + ex.Message);
        }
        return list;
    }

    public override void AppOpenAdLoad()
    {
        CallStatic("appOpenAdLoad");
    }

    public override void AppOpenAdStart(string pname, Action<int, string, string> action)
    {
        var cb = new AndroidCallback<string>(action, ParseString);
        CallStatic("appOpenAdStart", pname, cb);
    }

    public override void AppOpenAdStop()
    {
        CallStatic("appOpenAdStop");
    }

    public override void UpdateUser(string name, string photo, Action<int, string> action)
    {
        var cb = new AndroidCallbackNoData(action);
        CallStatic("updateUser", name, photo, cb);
    }

    public override MaxGetUserData GetUser()
    {
        var s = UnityPluginClass.CallStatic<string>("getUser");
        return MaxGetUserData.Parse(s);
    }

    public override void LbScore(int score)
    {
        CallStatic("lbScore", score);
    }

    public override void LbQueryInit(MaxLbQueryOptions options)
    {
        CallStatic("lbQueryInit", options.ToString());
    }

    public override void LbGlobalQuery(bool up, Action<int, string, List<MaxLbQueryData>> action)
    {
        var cb = new AndroidCallback<List<MaxLbQueryData>>(action, MaxLbQueryData.Parse);
        CallStatic("lbGlobalQuery", up, cb);
    }

    public override string GetPrivacyURL()
    {
        return UnityPluginClass.CallStatic<string>("getPrivacyURL");
    }

    public override string GetUserURL()
    {
        return UnityPluginClass.CallStatic<string>("getUserURL");
    }
}
#endif