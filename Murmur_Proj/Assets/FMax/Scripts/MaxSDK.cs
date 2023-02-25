

using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class MaxSDK :
#if UNITY_ANDROID
    MaxSDKAndroid
#elif UNITY_IOS || UNITY_IPHONE
    MaxSDKiOS
#else
    MaxSDKDefault
#endif
{
    private const string _sdkName = "FlowHwSDK";

    private const string _version = "1.2.1.3-6d83c456";

    private static readonly MaxSDK instance = new MaxSDK();

    public static MaxSDK Instance
    {
        get
        {
            return instance;
        }
    }

    public static string Version
    {
        get { return _version; }
    }

    public static string SdkName
    {
        get { return _sdkName; }
    }
}