using System;
using System.Collections.Generic;
using Max.ThirdParty;

public class MaxSDKDefault : MaxSDKBase
{
#if UNITY_ANDROID
    public static readonly int[] AdEncryptKey4 = { 13,64,29,180,35,122,246,25,224,100,47,199,228,232,72,243,147,119,138,70,209,127,225,83,44,73,56,226,76,0,56,156 };
#endif

    private Action<int, string, MaxChargeData> chargeAction;

    private void DoAction(Action action)
    {
        if (MaxSDKBehaviour.IsMainThread)
        {
            action();
        }
        else
        {
            MaxSDKBehaviour.QueueOnMainThread(action);
        }
    }

    public override void Init(MaxInitOptions option, Action<int, string, MaxInitData> action)
    {
        DoAction(() =>
        {
            var data = new MaxInitData();
            data.Os = "android";
            data.Did = "1234567890";
            data.Appv = option.Appv;
            data.Pkgv = "1.0.0";
            data.Sdkv = MaxSDK.Version;
            data.LoginTypes = "guest";
            data.Debug = true;
            data.Screen = false;
            data.Cki = false;
            action(1, "success", data);
        });
    }

    public override void AbtestConfig(Action<int, string, Dictionary<string, string>> action)
    {
        DoAction(() =>
        {
            action(1, "success", new Dictionary<string, string>());
        });
    }

    public override void PushEvent(string _event, Dictionary<string, string> data)
    {
        //do nothing
    }

    public override void Login(Action<int, string, MaxLoginData> loginAction, Action<int, string, MaxChargeData> chargeAction)
    {
        if (chargeAction != null)
        {
            this.chargeAction = chargeAction;
        }
        DoAction(() =>
        {
            var data = new MaxLoginData();
            data.Uid = "qwertyuiop";
            data.Sid = "";
            data.DisplayName = "";
            data.PhotoUrl = "";
            data.RegTime = "1663555613000";
            data.Country = "CN";
            data.Region = "sg";
            loginAction(1, "success", data);

        });
    }

    public override void RewardAdPreload(string unionId, Action<int, string, string> action)
    {
        DoAction(() =>
        {
            action(1, "success", "");
        });
    }

    public override void RewardAdShow(string unionId, string pname, Action<int, string, string> action)
    {
        DoAction(() =>
        {
            action(1, "success", "");
        });
    }

    public override void FullVideoAdPreload(string unionId)
    {
        //do nothing
    }

    public override void FullVideoAdShow(string unionId, string pname, bool skipLoading, Action<int, string, string> action)
    {
        DoAction(() =>
        {
            action(1, "success", "");
        });
    }

    public override int BannerAdFitHeightPx()
    {
        return 50;
    }

    public override void BannerAdInit(List<MaxBannerOptions> options)
    {
        //do nothing
    }

    public override void BannerAdShow(List<string> pnames)
    {
        //do nothing
    }

    public override void Charge(string productId, string consumeExt = null)
    {
        DoAction(() =>
        {
            if (chargeAction != null)
            {
                var data = new MaxChargeData();
                data.OrderSn = "";
                data.ConsumeExt = consumeExt;
                data.ProductId = productId;
                data.ProductType = 1;
                data.Expires = 0;
                data.IsEffect = false;
                chargeAction(1, "success", data);
            }
        });
    }

    public override void ChargeFinish(string orderSn)
    {
        //do nothing
    }

    public override void ChargeProducts(List<string> productIds, Action<int, string, List<MaxChargeProductData>> action)
    {
        DoAction(() =>
        {
            action(0, "default平台不支持", null);
        });
    }

    public override void ChargeRecovery()
    {
        //do nothing
    }

    public override void RequestReview(Action<int, string> action)
    {
        DoAction(() =>
        {
            action(1, "success");
        });
    }

    public override void SocialFrom(Action<int, string, MaxSocialInviteFromData> action)
    {
        DoAction(() =>
        {
            var data = new MaxSocialInviteFromData();
            data.From = 0;
            data.Name = "";
            data.Ext = "";
            action(1, "success", data);
        });
    }

    public override void SocialInvite(string type, string name, string ext, Action<int, string> action)
    {
        DoAction(() =>
        {
            action(1, "success");
        });
    }

    public override void SocialInviteCount(string name, Action<int, string, int> action)
    {
        DoAction(() =>
        {
            action(1, "success", 0);
        });
    }

    public override void SocialInviteReward()
    {
        //do nothing
    }

    public override void CustomLoginToken(Action<int, string, MaxLoginData> loginAction, Action<int, string, MaxChargeData> chargeAction)
    {
        if (chargeAction != null)
        {
            this.chargeAction = chargeAction;
        }
        DoAction(() =>
        {
            var data = new MaxLoginData();
            data.Uid = "qwertyuiop";
            data.Sid = "";
            data.DisplayName = "";
            data.PhotoUrl = "";
            data.RegTime = "1663555613000";
            data.Country = "CN";
            data.Region = "sg";
            loginAction(1, "success", data);
        });
    }

    public override void CustomLogin(string type, Action<int, string, MaxLoginData> loginAction, Action<int, string, MaxChargeData> chargeAction)
    {
        if (chargeAction != null)
        {
            this.chargeAction = chargeAction;
        }

        DoAction(() =>
        {
            var data = new MaxLoginData();
            data.Uid = "qwertyuiop";
            data.Sid = "";
            data.DisplayName = "";
            data.PhotoUrl = "";
            data.RegTime = "1663555613000";
            data.Country = "CN";
            data.Region = "sg";
            loginAction(1, "success", data);
        });
    }

    public override void Logout(Action<int, string> action)
    {
        DoAction(() =>
        {
            action(1, "success");
        });
    }

    public override void Link(string type, bool shouldSwitch, Action<int, string, MaxLoginData> action)
    {
        DoAction(() =>
        {
            action(1, "success", null);
        });
    }

    public override List<string> getLinkedList()
    {
        List<string> list = new List<string>();
        list.Add("fb");
        return list;
    }

    public override void AppOpenAdLoad()
    {
        // do nothing
    }

    public override void AppOpenAdStart(string pname, Action<int, string, string> action)
    {
        DoAction(() =>
        {
            action(1, "success", "unionId-123456");
        });
    }

    public override void AppOpenAdStop()
    {
        // do nothing
    }

    public override void ChargeSetCallback(Action<int, string, MaxChargeData> chargeAction)
    {
        if (chargeAction != null)
        {
            this.chargeAction = chargeAction;
        }
    }

    public override void UpdateUser(string name, string photo, Action<int, string> action)
    {
        DoAction(() =>
        {
            action(1, "success");
        });
    }

    public override MaxGetUserData GetUser()
    {
        var data = new MaxGetUserData();
        data.DisplayName = "";
        data.PhotoUrl = "";
        data.Region = "";
        data.Country = "";
        return data;
    }

    public override void LbScore(int score)
    {
        //do nothing
    }

    public override void LbQueryInit(MaxLbQueryOptions options)
    {
        //do nothing
    }

    public override void LbGlobalQuery(bool up, Action<int, string, List<MaxLbQueryData>> action)
    {
        DoAction(() =>
        {
            action(1, "success", new List<MaxLbQueryData>());
        });
    }

    public override string GetPrivacyURL()
    {
        return "https://www.google.com";
    }

    public override string GetUserURL()
    {
        return "https://www.google.com";
    }
}

