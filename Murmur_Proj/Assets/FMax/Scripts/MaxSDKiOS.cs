#if UNITY_IOS || UNITY_IPHONE

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.InteropServices;
using Max.ThirdParty;
using UnityEngine;

public class MaxSDKiOS : MaxSDKBase
{
    private static ConcurrentDictionary<long, Action<int, string, string>> callbackDict = new ConcurrentDictionary<long, Action<int, string, string>>();
    private static long callbackCounter = 0;

    private static long pushCallback(Action<int, string, string> action)
    {
        var idx = Interlocked.Increment(ref callbackCounter);
        callbackDict[idx] = action;
        return idx;
    }

    delegate void iosCallback(long idx, int ret, string msg, string data);

    [AOT.MonoPInvokeCallback(typeof(iosCallback))]
    static void iosCallbackHandler(long idx, int ret, string msg, string data)
    {
        Action<int, string, string> action;
        if (callbackDict.TryRemove(idx, out action) && action != null)
        {
            action(ret, msg, data);
        }
    }

    private static Action<int, string, MaxChargeData> iosChargeAction = null;

    [AOT.MonoPInvokeCallback(typeof(iosCallback))]
    static void iosChargeCallbackHandler(long idx, int ret, string msg, string data)
    {
        if (iosChargeAction != null)
        {
            iosChargeAction(ret, msg, MaxChargeData.Parse(data));
        }
    }

    [DllImport("__Internal")]
    static extern void Flow998_init(long idx, iosCallback callback, string options);

    public override void Init(MaxInitOptions option, Action<int, string, MaxInitData> action)
    {
        var idx = pushCallback((int ret, string msg, string data) =>
        {
            action(ret, msg, MaxInitData.Parse(data));
        });
        Flow998_init(idx, iosCallbackHandler, option.ToString());
    }

    [DllImport("__Internal")]
    static extern void Flow998_abtestConfig(long idx, iosCallback callback);

    public override void AbtestConfig(Action<int, string, Dictionary<string, string>> action)
    {
        var idx = pushCallback((int ret, string msg, string data) =>
        {
            UnityEngine.Debug.Log("AbtestConfig callback - " + (data != null ? data : "null"));
            action(ret, msg, ParseDictionary(data));
        });
        Flow998_abtestConfig(idx, iosCallbackHandler);
    }

    [DllImport("__Internal")]
    static extern void Flow998_pushEvent(string _event, string data);

    public override void PushEvent(string _event, Dictionary<string, string> data)
    {
        Flow998_pushEvent(_event, Json.Serialize(data));
    }

    [DllImport("__Internal")]
    static extern void Flow998_login(long loginIdx, iosCallback loginCallback, iosCallback chargeCallback);

    public override void Login(Action<int, string, MaxLoginData> loginAction, Action<int, string, MaxChargeData> chargeAction)
    {
        var loginIdx = pushCallback((int ret, string msg, string data) =>
        {
            loginAction(ret, msg, MaxLoginData.Parse(data));
        });
        if (chargeAction == null)
        {
            Flow998_login(loginIdx, iosCallbackHandler, (iosCallback)null);
        }
        else
        {
            iosChargeAction = chargeAction;
            Flow998_login(loginIdx, iosCallbackHandler, iosChargeCallbackHandler);
        }
    }

    [DllImport("__Internal")]
    static extern void Flow998_rewardAdPreload(long idx, iosCallback callback, string unionId);

    public override void RewardAdPreload(string unionId, Action<int, string, string> action)
    {
        var idx = pushCallback((int ret, string msg, string data) =>
        {
            action(ret, msg, ParseString(data));
        });
        Flow998_rewardAdPreload(idx, iosCallbackHandler, "");
    }

    [DllImport("__Internal")]
    static extern void Flow998_rewardShow(long idx, iosCallback callback, string unionId, string pname);

    public override void RewardAdShow(string unionId, string pname, Action<int, string, string> action)
    {
        var idx = pushCallback((int ret, string msg, string data) =>
        {
            action(ret, msg, ParseString(data));
        });
        Flow998_rewardShow(idx, iosCallbackHandler, "", pname);
    }

    [DllImport("__Internal")]
    static extern void Flow998_fullVideoAdPreload(string unionId);

    public override void FullVideoAdPreload(string unionId)
    {
        Flow998_fullVideoAdPreload("");
    }

    [DllImport("__Internal")]
    static extern void Flow998_fullVideoAdShow(long idx, iosCallback callback, string unionId, string pname);

    public override void FullVideoAdShow(string unionId, string pname, bool skipLoading, Action<int, string, string> action)
    {
        var idx = pushCallback((int ret, string msg, string data) =>
        {
            action(ret, msg, ParseString(data));
        });
        Flow998_fullVideoAdShow(idx, iosCallbackHandler, "", pname);
    }


    [DllImport("__Internal")]
    static extern int Flow998_bannerAdFitHeightPx();

    public override int BannerAdFitHeightPx()
    {
        return Flow998_bannerAdFitHeightPx();
    }

    [DllImport("__Internal")]
    static extern void Flow998_bannerAdInit(string data);

    public override void BannerAdInit(List<MaxBannerOptions> options)
    {
        var list = new List<Dictionary<string, object>>();
        foreach (var option in options)
        {
            list.Add(option.ToDictionary());
        }
        Flow998_bannerAdInit(Json.Serialize(list));
    }

    [DllImport("__Internal")]
    static extern void Flow998_bannerAdShow(string data);

    public override void BannerAdShow(List<string> pnames)
    {
        Flow998_bannerAdShow(Json.Serialize(pnames));
    }

    [DllImport("__Internal")]
    static extern void Flow998_charge(string productId, string consumeExt);

    public override void Charge(string productId, string consumeExt = null)
    {
        Flow998_charge(productId, consumeExt);
    }

    [DllImport("__Internal")]
    static extern void Flow998_chargeRecovery();

    public override void ChargeRecovery()
    {
        Flow998_chargeRecovery();
    }

    [DllImport("__Internal")]
    static extern void Flow998_chargeProducts(long idx, iosCallback callback, string productIds);

    public override void ChargeProducts(List<string> productIds, Action<int, string, List<MaxChargeProductData>> action)
    {
        var idx = pushCallback((int ret, string msg, string data) =>
        {
            UnityEngine.Debug.Log("ChargeProducts callback - " + (data != null ? data : "null"));
            action(ret, msg, MaxChargeProductData.Parse(data));
        });
        Flow998_chargeProducts(idx, iosCallbackHandler, Json.Serialize(productIds));
    }

    [DllImport("__Internal")]
    static extern void Flow998_chargeSetCallback(long idx, iosCallback chargeCallback);

    public override void ChargeSetCallback(Action<int, string, MaxChargeData> chargeAction)
    {
        iosChargeAction = chargeAction;
        Flow998_chargeSetCallback(0, iosChargeCallbackHandler);
    }

    [DllImport("__Internal")]
    static extern void Flow998_chanrgeFinish(string orderSn);

    public override void ChargeFinish(string orderSn)
    {
        Flow998_chanrgeFinish(orderSn);
    }

    [DllImport("__Internal")]
    static extern void Flow998_requestReview(long idx, iosCallback callback);

    public override void RequestReview(Action<int, string> action)
    {
        var idx = pushCallback((int ret, string msg, string data) =>
        {
            action(ret, msg);
        });
        Flow998_requestReview(idx, iosCallbackHandler);
    }

    [DllImport("__Internal")]
    static extern void Flow998_socialInvite(long idx, iosCallback callback, string type, string name, string ext);

    public override void SocialInvite(string type, string name, string ext, Action<int, string> action)
    {
        var idx = pushCallback((int ret, string msg, string data) =>
        {
            action(ret, msg);
        });
        Flow998_socialInvite(idx, iosCallbackHandler, type, name, ext);
    }

    [DllImport("__Internal")]
    static extern void Flow998_socialInviteCount(long idx, string name, iosCallback callback);

    public override void SocialInviteCount(string name, Action<int, string, int> action)
    {
        var idx = pushCallback((int ret, string msg, string data) =>
        {
            action(ret, msg, ParseInt(data));
        });
        Flow998_socialInviteCount(idx, name, iosCallbackHandler);
    }

    [DllImport("__Internal")]
    static extern void Flow998_socialFrom(long idx, iosCallback callback);

    public override void SocialFrom(Action<int, string, MaxSocialInviteFromData> action)
    {
        var idx = pushCallback((int ret, string msg, string data) =>
        {
            action(ret, msg, MaxSocialInviteFromData.Parse(data));
        });
        Flow998_socialFrom(idx, iosCallbackHandler);
    }

    [DllImport("__Internal")]
    static extern void Flow998_socialInviteReward();

    public override void SocialInviteReward()
    {
        Flow998_socialInviteReward();
    }

    [DllImport("__Internal")]
    static extern void Flow998_customLoginToken(long loginIdx, iosCallback loginCallback, iosCallback chargeCallback);

    public override void CustomLoginToken(Action<int, string, MaxLoginData> loginAction, Action<int, string, MaxChargeData> chargeAction)
    {
        var loginIdx = pushCallback((int ret, string msg, string data) =>
        {
            loginAction(ret, msg, MaxLoginData.Parse(data));
        });
        if (chargeAction == null)
        {
            Flow998_customLoginToken(loginIdx, iosCallbackHandler, null);
        }
        else
        {
            iosChargeAction = chargeAction;
            Flow998_customLoginToken(loginIdx, iosCallbackHandler, iosChargeCallbackHandler);
        }
    }

    [DllImport("__Internal")]
    static extern void Flow998_customLogin(long loginIdx, string type, iosCallback loginCallback, iosCallback chargeCallback);

    public override void CustomLogin(string type, Action<int, string, MaxLoginData> loginAction, Action<int, string, MaxChargeData> chargeAction)
    {
        var loginIdx = pushCallback((int ret, string msg, string data) =>
        {
            loginAction(ret, msg, MaxLoginData.Parse(data));
        });
        if (chargeAction == null)
        {
            Flow998_customLogin(loginIdx, type, iosCallbackHandler, null);
        }
        else
        {
            iosChargeAction = chargeAction;
            Flow998_customLogin(loginIdx, type, iosCallbackHandler, iosChargeCallbackHandler);
        }
    }

    [DllImport("__Internal")]
    static extern void Flow998_logout(long idx, iosCallback callback);

    public override void Logout(Action<int, string> action)
    {
        var idx = pushCallback((int ret, string msg, string data) =>
        {
            action(ret, msg);
        });
        Flow998_logout(idx, iosCallbackHandler);
    }


    [DllImport("__Internal")]
    static extern void Flow998_link(long idx, string type, bool shouldSwitch, iosCallback callback);
    public override void Link(string type, bool shouldSwitch, Action<int, string, MaxLoginData> action)
    {
        var loginIdx = pushCallback((int ret, string msg, string data) =>
        {
            action(ret, msg, MaxLoginData.Parse(data));
        });
        Flow998_link(loginIdx, type, shouldSwitch, iosCallbackHandler);
    }

    [DllImport("__Internal")]
    static extern string Flow998_getLinklist();

    public override List<string> getLinkedList()
    {
        List<string> list = new List<string>();
        var str = Flow998_getLinklist();
        if (str == null || str.Length == 0)
        {
            return list;
        }
        var arr = str.Split(',');
        foreach (var s in arr)
        {
            list.Add(s);
        }
        return list;
    }

    [DllImport("__Internal")]
    static extern void Flow998_appAdOpenAdLoad();

    public override void AppOpenAdLoad()
    {
        Flow998_appAdOpenAdLoad();
    }

    [DllImport("__Internal")]
    static extern void Flow998_appOpenAdStart(long idx, string pname, iosCallback callback);

    public override void AppOpenAdStart(string pname, Action<int, string, string> action)
    {
        var idx = pushCallback((int ret, string msg, string data) =>
        {
            action(ret, msg, ParseString(data));
        });
        Flow998_appOpenAdStart(idx, pname, iosCallbackHandler);
    }

    [DllImport("__Internal")]
    static extern void Flow998_appOpenAdStop();

    public override void AppOpenAdStop()
    {
        Flow998_appOpenAdStop();
    }

    [DllImport("__Internal")]
    static extern void Flow998_updateUser(long idx, string name, string photo, iosCallback callback);

    public override void UpdateUser(string name, string photo, Action<int, string> action)
    {
        var idx = pushCallback((int ret, string msg, string data) =>
        {
            action(ret, msg);
        });
        Flow998_updateUser(idx, name, photo, iosCallbackHandler);
    }

    [DllImport("__Internal")]
    static extern string Flow998_getUser();

    public override MaxGetUserData GetUser()
    {
        var s = Flow998_getUser();
        return MaxGetUserData.Parse(s);
    }

    [DllImport("__Internal")]
    static extern void Flow998_lbScore(int score);

    public override void LbScore(int score)
    {
        Flow998_lbScore(score);
    }

    [DllImport("__Internal")]
    static extern void Flow998_lbQueryInit(string options);

    public override void LbQueryInit(MaxLbQueryOptions options)
    {
        Flow998_lbQueryInit(options.ToString());
    }

    [DllImport("__Internal")]
    static extern void Flow998_lbGlobalQuery(long idx, bool up, iosCallback callback);

    public override void LbGlobalQuery(bool up, Action<int, string, List<MaxLbQueryData>> action)
    {
        var idx = pushCallback((int ret, string msg, string data) =>
        {
            action(ret, msg, MaxLbQueryData.Parse(data));
        });
        Flow998_lbGlobalQuery(idx, up, iosCallbackHandler);
    }

    [DllImport("__Internal")]
    static extern string Flow998_getPrivacyURL();

    public override string GetPrivacyURL()
    {
        return Flow998_getPrivacyURL();
    }

    [DllImport("__Internal")]
    static extern string Flow998_getUserURL();

    public override string GetUserURL()
    {
        return Flow998_getUserURL();
    }
}

#endif