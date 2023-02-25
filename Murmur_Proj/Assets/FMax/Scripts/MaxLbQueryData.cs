using System;
using System.Collections.Generic;
using Max.ThirdParty;
using UnityEngine;

public class MaxLbQueryData
{
    public string DisplayName; //用户昵称，可能为空字符串
    public string PhotoUrl; //用户头像，可能为空字符串
    public int Score; //用户分数
    public string Country; //用户国家，可能为空字符串
    public bool Current; //该记录是否是当前用户

    public static List<MaxLbQueryData> Parse(string s)
    {

        if (s == null || s.Length == 0)
        {
            return new List<MaxLbQueryData>();
        }
        var ret = new List<MaxLbQueryData>();
        try
        {
            var list = Json.Deserialize(s) as List<object>;
            foreach (var v in list)
            {
                var dict = Json.Deserialize(s) as Dictionary<string, object>;
                var data = new MaxLbQueryData();
                data.DisplayName = dict["displayName"].ToString();
                data.PhotoUrl = dict["photoUrl"].ToString();
                data.Score = (int)(long)dict["score"];
                data.Country = dict["country"].ToString();
                data.Current = (bool)dict["current"];
                ret.Add(data);
            }
        }
        catch (Exception ex)
        {
            Debug.Log("MaxLbQueryData exception - " + ex.Message);
        }
        return ret;
    }
}

