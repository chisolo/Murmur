
using System;
using System.Collections.Generic;
using Max.ThirdParty;
using UnityEngine;


public class MaxGetUserData
{
    public string Uid = ""; //用户id，为空字符串，则表示当前未登录
	public string DisplayName = ""; //用户昵称，可能为空字符串
    public string PhotoUrl = ""; //用户头像，可能为空字符串
    public string Country = ""; //用户注册所在国家
    public string Region = ""; //用户注册就近的服务端区域

    public static MaxGetUserData Parse(string s)
    {
        var data = new MaxGetUserData();
        if (s == null || s.Length == 0)
        {
            return data;
        }
        try
        {
            var dict = Json.Deserialize(s) as Dictionary<string, object>;
            data.Uid = dict["uid"].ToString();
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
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log("MaxGetUserData exception - " + ex.Message);
        }
        return data;
    }
}

