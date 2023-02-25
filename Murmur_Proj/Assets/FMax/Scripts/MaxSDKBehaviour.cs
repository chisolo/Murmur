using UnityEngine;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;
using System.Threading;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.TextCore;

public class MaxSDKBehaviour : MonoBehaviour
{
    private static MaxSDKBehaviour _current;

    public static MaxSDKBehaviour Current
    {
        get
        {
            Initialize();
            return _current;
        }
    }

    static bool initialized;

    static void Initialize()
    {
        if (!initialized)
        {
            if (!Application.isPlaying)
                return;
            initialized = true;
            var g = new GameObject(MaxSDK.SdkName);
            _current = g.AddComponent<MaxSDKBehaviour>();
            UnityEngine.Object.DontDestroyOnLoad(_current.gameObject);
        }
    }

    public static bool IsMainThread
    {
        get { return Current.threadId == System.Threading.Thread.CurrentThread.ManagedThreadId; }
    }

    public static void QueueOnMainThread(Action action)
    {
        lock (Current._actions)
        {
            Current._actions.Add(action);
        }
    }

#if UNITY_ANDROID
    public static readonly int[] AdEncryptKey1 = { 102,156,96,226,205,118,65,11,144,43,206,66,233,78,60,218,85,113,68,218,36,253,223,239,243,170,214,35,126,150,127,159 };

    [DllImport("FlowHwSDK")]
    private static extern void Flow998_ad_show(int type, string unionId, string pname, string index);

    private static ConcurrentDictionary<string, Action<int, string, string>> adCallbackDict = new ConcurrentDictionary<string, Action<int, string, string>>();
    private static long adCallbackCounter = 0;

    public void OnAdCallback(string data)
    {
        //{ret},{type},{data},{encrypted_data}
        var arr = data.Split(',');
        if (arr.Length != 4)
        {
            Debug.Log("OnAdCallback data invalid - " + data);
            return;
        }
        var ret = arr[0];
        var type = arr[1];
        var index = arr[2];
        var encrypted_index = arr[3];

        Action<int, string, string> action;
        if (!adCallbackDict.TryRemove(index, out action) || action == null)
        {
            Debug.Log("OnAdCallback action not found - " + data);
            return;
        }
        //ad encrypt
        byte[] input = Encoding.UTF8.GetBytes(index);
        byte[] output = new byte[input.Length];
        int j = 0, k, output_index = 0;
        for (int i = 0; i < input.Length; i++)
        {
            j = (j + input[i]) % AdEncryptKey1.Length;
            output[i] = (byte)(AdEncryptKey1[j] ^ input[i]);
        }
        for (int i = input.Length - 1; i >= 0; i--)
        {
            j = (j + output[i]) % MaxLoginData.AdEncryptKey2.Length;
            output[i] = (byte)(MaxLoginData.AdEncryptKey2[j] ^ output[i]);
            if (i == 3)
            {
                goto encrypt4;
            }
        }
    encrypt3:
        for (int i = 0; i < MaxBannerOptions.AdEncryptKey3.Length; i++)
        {
            k = (output_index++) % input.Length;
            j = (j + MaxBannerOptions.AdEncryptKey3[i]) % MaxBannerOptions.AdEncryptKey3.Length;
            output[k] = (byte)(MaxBannerOptions.AdEncryptKey3[j] ^ output[k]);
        }
        goto encrypt_end;
    encrypt4:
        for (int i = MaxSDKDefault.AdEncryptKey4.Length - 1; i >= 0; i--)
        {
            k = (output_index++) % input.Length;
            output[k] = (byte)(MaxSDKDefault.AdEncryptKey4[i] ^ output[k]);
            if (i == 2)
            {
                goto encrypt3;
            }
        }

    encrypt_end:
        var encrypted_index1 = Convert.ToBase64String(output);
        if (encrypted_index != encrypted_index1)
        {
            Debug.Log("OnAdCallback encrypt valid");
            return;
        }
        if (ret == "1")
        {
            action(1, "success", "");
        }
        else
        {
            action(0, "error", "");
        }
    }

    public void ShowAd(int type, string unionId, string pname, Action<int, string, string> action)
    {
        var idx = Interlocked.Increment(ref adCallbackCounter);
        var md5 = MD5.Create();
        var salt = UnityEngine.Random.Range(10000, 10000000).ToString();
        var index = Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(idx.ToString() + "@" + salt)));
        adCallbackDict[index] = action;
        //需要切换到主线程调用
        if (MaxSDKBehaviour.IsMainThread)
        {
            Flow998_ad_show(type, unionId, pname, index);
        }
        else
        {
            MaxSDKBehaviour.QueueOnMainThread(() =>
            {
                Flow998_ad_show(type, unionId, pname, index);
            });
        }
    }
#endif

    private int threadId;
    private List<Action> _actions = new List<Action>();

    void Awake()
    {
        _current = this;
        initialized = true;
    }

    void OnDisable()
    {
        if (_current == this)
        {
            _current = null;
        }
    }

    // Use this for initialization
    void Start()
    {
        threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
    }

    List<Action> _currentActions = new List<Action>();

    // Update is called once per frame
    void Update()
    {
        lock (_actions)
        {
            _currentActions.Clear();
            _currentActions.AddRange(_actions);
            _actions.Clear();
        }
        foreach (var a in _currentActions)
        {
            a();
        }
    }
}