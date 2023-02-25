
using System;
using System.Collections.Generic;
using Max.ThirdParty;

public class MaxLoginData
{
#if UNITY_ANDROID
    public static readonly int[] AdEncryptKey2 = { 19,97,179,213,214,10,20,188,47,110,86,188,28,76,58,21 };
#endif


    public string Uid; //用户
    public string Sid; //服务端二次校验的sid，暂时无用
    public string DisplayName; //用户昵称，可能为空字符串
    public string PhotoUrl; //用户头像，可能为空字符串
    public string RegTime; //用户注册时间戳，单位毫秒
    public string Country; //如果初始化使用了UseRegion=true，则保证返回用户所在国家
    public string Region; //如果初始化使用了UseRegion=true，则保证返回用户就近的服务端区域

    public static MaxLoginData Parse(string s)
    {
        if (s == null || s.Length == 0)
        {
            return null;
        }
        try
        {
            var dict = Json.Deserialize(s) as Dictionary<string, object>;
            var data = new MaxLoginData();
            data.Uid = dict["uid"].ToString();
            data.Sid = dict["sid"].ToString();
            data.DisplayName = dict["displayName"].ToString();
            data.PhotoUrl = dict["photoUrl"].ToString();
            var country = dict.ContainsKey("country") ? dict["country"] : null;
            if (country == null)
            {
                data.Country = string.Empty;
            }
            else
            {
                data.Country = country.ToString();
            }
            var region = dict.ContainsKey("region") ? dict["region"] : null;
            if (region == null)
            {
                data.Region = string.Empty;
            }
            else
            {
                data.Region = region.ToString();
            }
            return data;
        } catch(Exception ex)
        {
            UnityEngine.Debug.Log("MaxLoginData exception - " + ex.Message);
        }
        return null;
    }
}