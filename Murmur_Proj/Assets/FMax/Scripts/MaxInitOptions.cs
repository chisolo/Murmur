
using System;
using System.Collections.Generic;
using Max.ThirdParty;

public class MaxInitOptions
{
    public string Appv; //游戏内部版本号
    public bool HideSdkPrivacy = false; //是否隐藏sdk的隐私弹框，暂时无用；
    public bool UseRegion = false; //游戏是否在login回调时有region/country字段；

    public MaxInitOptions(string appv)
    {
        this.Appv = appv;
    }

    public MaxInitOptions(string appv, bool hideSdkPrivacy, bool useRegion)
    {
        this.Appv = appv;
        this.HideSdkPrivacy = hideSdkPrivacy;
        this.UseRegion = useRegion;
    }

    public override string ToString()
    {
        var dict = new Dictionary<string, object>();
        dict.Add("appv", Appv);
        dict.Add("hideSdkPrivacy", HideSdkPrivacy);
        dict.Add("useRegion", UseRegion);
        return Json.Serialize(dict);
    }
}