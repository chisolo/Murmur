
using System;
using System.Collections.Generic;
using Max.ThirdParty;
using UnityEngine;

public class MaxInitData
{
    public string Os; //系统 = android or ios
    public string Did; //设备号
    public string Appv; //游戏内部版本号
    public string Pkgv; //包体版本号
    public string Sdkv; //sdk版本号
    public string LoginTypes; //登录方式列表，暂时无用
    public bool Debug; //是否为debug模式
    public bool Screen; //是否是录屏包
    public bool Cki; //true则表示ios审核期间

    public static MaxInitData Parse(string s)
    {
        if (s == null || s.Length == 0)
        {
            return null;
        }
        try
        {
            var dict = Json.Deserialize(s) as Dictionary<string, object>;
            var data = new MaxInitData();
            data.Os = dict["os"].ToString();
            data.Did = dict["did"].ToString();
            data.Appv = dict["appv"].ToString();
            data.Pkgv = dict["pkgv"].ToString();
            data.Sdkv = dict["sdkv"].ToString();
            data.LoginTypes = dict["login_types"].ToString();
            data.Debug = (bool)dict["debug"];
            data.Screen = (bool)dict["screen"];
            data.Cki = (bool)dict["cki"];
            return data;
        } catch(Exception ex)
        {
            UnityEngine.Debug.Log("MaxInitData exception - " + ex.Message);
        }
        return null;
    }
}