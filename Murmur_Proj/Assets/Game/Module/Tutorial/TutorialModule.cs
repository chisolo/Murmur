using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Lemegeton;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TutorialModule : Singleton<TutorialModule>
{
    TutorialModule() {}
    private List<TutorialConfig> _tutorialConfig;
    private TutorialArchive _tutorialArchive;
    private List<TutorialData> _tutorialDatas;
    private Dictionary<string, Func<TutorialData, bool>> _triggers;
    private Dictionary<string, Func<TutorialData, bool>> _checks;
    private TutorialData _curTutorial;
    private bool _inited = false;
    private int _idx = 0;
    public void Init()
    {
        if(_inited) return;
        _tutorialConfig = ConfigModule.Instance.Tutorials();
        _tutorialArchive = ArchiveModule.Instance.GetArchive<TutorialArchive>();
        _tutorialDatas = new List<TutorialData>();
        _curTutorial = null;
        _triggers = new Dictionary<string, Func<TutorialData, bool>>()
        {
            {"EnterGame", EnterGameTrigger},
            {"TutorialFinish", TutorialFinishTrigger},
            {"MoneyFull", MoneyFullTrigger},
            {"BuildingLevel", BuildingLevelTrigger},
            {"EnhanceLevelMax", EnhanceLevelMaxTrigger},
            {"SausageDelivered", SausageDeliveredTrigger}
        };
        _checks = new Dictionary<string, Func<TutorialData, bool>>()
        {
            {"BuildingLevel", BuildingLevelCheck},
        };
        foreach(var tutorialConfig in _tutorialConfig) {
            if(_tutorialArchive.IsComplete(tutorialConfig.id)) continue;
            var tutorialData = new TutorialData(tutorialConfig);
            _tutorialDatas.Add(tutorialData);
        }
        _inited = true;
    }

    public void OnLoad()
    {
#if UNITY_EDITOR
        // TODO
        var isChecked = EditorPrefs.GetBool("d_SkipTutorial", false);
        //Debug.Log("skip tutoail " + isChecked);
        if (isChecked) {
            return;
        }
#endif

        StartCoroutine(UpdateCoroutine());
    }
    public bool IsComplete(string tutorialId)
    {
        return _tutorialArchive.IsComplete(tutorialId);
    }
    private IEnumerator UpdateCoroutine()
    {
        while(true) {
            yield return new WaitForSeconds(0.5f);
            if(_tutorialDatas.Count > 0 && _curTutorial == null) {
                foreach(var tutorial in _tutorialDatas) {
                    if(!string.IsNullOrEmpty(tutorial.config.check)) {
                        var check = _checks[tutorial.config.check];
                        if(check.Invoke(tutorial)) {
                            _tutorialArchive.SetData(tutorial.config.id);
                            tutorial.done = true;
                        }
                    }
                    if(tutorial.done) continue;
                    var trigger = _triggers[tutorial.config.trigger];
                    if(trigger.Invoke(tutorial)) {
                        _curTutorial = tutorial;
                        using (var arg  = TutorialStartEventArgs.Get()) {
                            arg.tutorialData = tutorial;
                            EventModule.Instance.FireEvent(EventDefine.TutorialStart, arg);
                        }
                        break;
                    }
                }
                for(int i = _tutorialDatas.Count-1; i >= 0; --i) {
                    if(_tutorialDatas[i].done) {
                        _tutorialDatas.RemoveAt(i);
                    }
                }
            }
        }
    }
    public void FinishTutorial()
    {
        TraceModule.Instance.TraceTutorial(_curTutorial.config.id);
        _tutorialDatas.Remove(_curTutorial);
        _curTutorial = null;
    }
    public void TutorialDone()
    {
        _tutorialArchive.SetData(_curTutorial.config.id);
        ArchiveModule.Instance.SaveArchive(_tutorialArchive);
    }

    private bool EnterGameTrigger(TutorialData tutorialData)
    {
        return true;
    }
    private bool TutorialFinishTrigger(TutorialData tutorialData)
    {
        return _tutorialArchive.IsComplete(tutorialData.config.string_trigger_param);
    }
    private bool MoneyFullTrigger(TutorialData tutorialData)
    {
        return PlayerModule.Instance.IsMoneyFull();
    }
    private bool BuildingLevelTrigger(TutorialData tutorialData)
    {
        return BuildingModule.Instance.GetBuildingLevel(tutorialData.config.string_trigger_param) >= tutorialData.config.int_trigger_param;
    }
    private bool EnhanceLevelMaxTrigger(TutorialData tutorialData)
    {
        return BuildingModule.Instance.IsBuildingEnhanceMax(tutorialData.config.string_trigger_param);
    }
    private bool SausageDeliveredTrigger(TutorialData tutorialData)
    {
        return BuildingModule.Instance.SausageDeliverd >= tutorialData.config.int_trigger_param;
    }
    private bool BuildingLevelCheck(TutorialData tutorialData)
    {
        return BuildingModule.Instance.GetBuildingLevel(tutorialData.config.string_check_param) >= tutorialData.config.int_check_param;
    }

}

