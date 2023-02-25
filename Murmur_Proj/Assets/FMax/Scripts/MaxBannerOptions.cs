
using System;
using UnityEngine;
using System.Collections.Generic;

public class MaxBannerOptions
{
#if UNITY_ANDROID
    public static readonly int[] AdEncryptKey3 = { 31,254,129,116,224,161,26,121,48,31,118,50,41,0,23,46,230,49,122,30,225,142,233,11,92,98,71,84,182,49,72,177 };
#endif

    public enum BannerPosition
    {
        BottomCenter = 0,
        TopCanter = 1
    }

    public string Pname;
    public string UnionId;
    public int? Color = null;
    public BannerPosition Position = BannerPosition.BottomCenter;

    public MaxBannerOptions(string pname)
    {
        this.Pname = pname;
    }

    [Obsolete("not support unionId", false)]
    public MaxBannerOptions(string pname, string unionId)
    {
        this.Pname = pname;
        this.UnionId = unionId;
    }

    public void SetColor(Color color)
    {
        int r = (int)(Mathf.Clamp01(color.r) * Byte.MaxValue);
        int g = (int)(Mathf.Clamp01(color.g) * Byte.MaxValue);
        int b = (int)(Mathf.Clamp01(color.b) * Byte.MaxValue);
        this.Color = (r << 16) + (g << 8) + b;
    }

    public Dictionary<string, object> ToDictionary()
    {
        var dict = new Dictionary<string, object>();
        dict.Add("pname", Pname);
        dict.Add("color", Color);
        dict.Add("position", Position);
        return dict;
    }
}