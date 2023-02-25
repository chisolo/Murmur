
using System;
using System.Collections.Generic;
using Max.ThirdParty;

public class MaxSocialInviteFromData
{
    public int From;//0=不是邀请用户; 1=邀请; 2=分享
    public string Name;//邀请点位
    public string Ext;//透传参数

    public static MaxSocialInviteFromData Parse(string s)
    {
        if (s == null || s.Length == 0)
        {
            return null;
        }
        try
        {
            var dict = Json.Deserialize(s) as Dictionary<string, object>;
            var data = new MaxSocialInviteFromData();
            data.From = (int)(long)dict["from"];
            var name = dict.ContainsKey("name") ? dict["name"] : null;
            if (name == null)
            {
                data.Name = string.Empty;
            }
            else
            {
                data.Name = name.ToString();
            }
            var ext = dict.ContainsKey("ext") ? dict["ext"] : null;
            if (ext == null)
            {
                data.Ext = String.Empty;
            }
            else
            {
                data.Ext = ext.ToString();
            }
            return data;
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log("MaxSocialInviteFromData exception - " + ex.Message);
        }
        return null;
    }
}