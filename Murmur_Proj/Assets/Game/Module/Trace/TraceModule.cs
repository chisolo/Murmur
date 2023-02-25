using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Lemegeton;

public class TraceModule : Singleton<TraceModule>
{
    TraceModule() {}

    public void Init()
    {
    }

    public void TraceQuest(string questId)
    {
    #if UNITY_EDITOR
        AppLogger.Log("{[Trace] task_complete : task_id = " + questId + "}");
    #else
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("tab_a", questId);
        MaxSDK.Instance.PushEvent("task_complete", data);
    #endif
    }

    public void TraceTalent(string talentId)
    {
    #if UNITY_EDITOR
        AppLogger.Log("{[Trace] research_complete : research_id = " + talentId + "}");
    #else
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("tab_a", talentId);
        MaxSDK.Instance.PushEvent("research_complete", data);
    #endif
    }

    public void TraceBuild(string buildId, int level)
    {
    #if UNITY_EDITOR
        AppLogger.Log("{[Trace] build_complete : build_id = " + buildId + ", build_level = " + level.ToString() + "}");
    #else
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("tab_a", buildId);
        data.Add("tab_b", level.ToString());
        MaxSDK.Instance.PushEvent("build_complete", data);
    #endif
    }

    public void TraceTutorial(string tutorialId)
    {
    #if UNITY_EDITOR
        AppLogger.Log("{[Trace] guide : guide_id = " + tutorialId + "}");
    #else
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("tab_a", tutorialId);
        MaxSDK.Instance.PushEvent("guide", data);
    #endif
    }
}

