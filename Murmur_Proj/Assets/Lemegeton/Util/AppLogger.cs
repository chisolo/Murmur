using UnityEngine;

public static class AppLogger
{
    public static void Verbose()
    {

    }

    [System.Diagnostics.Conditional("IS_DEV"), System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Log(object message)
    {
        Debug.Log(message);
    }

    [System.Diagnostics.Conditional("IS_DEV"), System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogFormat(string format, params object[] args)
    {
        Debug.LogFormat(format, args);
    }

    public static void LogWarning(object message)
    {
        Debug.LogWarning(message);
    }


    public static void LogError(object message)
    {
        Debug.LogError(message);
    }
}