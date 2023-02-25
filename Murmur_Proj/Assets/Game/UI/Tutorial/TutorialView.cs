using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class TutorialView : PopupUIBaseCtrl
{
    public class Args : PopupUIArgs
    {
        public TutorialData tutorialData;
    }
    public static string PrefabPath = "Assets/Res/UI/Prefab/Tutorial/TutorialView.prefab";

    [SerializeField] GameObject _dialogObj;
    [SerializeField] Text _dialogTxt;
    [SerializeField] Button _dialogBtn;
    [SerializeField] GameObject _handObj;
    [SerializeField] HoleMask _holeMask;
    [SerializeField] GameObject _clickTip;

    private Dictionary<string, Action> _stepActions;
    private TutorialData _tutorialData;
    private TweenerCore<string, string, StringOptions> _dialogAction;
    private bool _hasTarget;
    private bool _hasDialog;
    private bool _dialogComplete;
    private bool _inStep;
    private bool _lock;
    public override void Init(PopupUIArgs arg)
    {
        _tutorialData = ((Args)arg).tutorialData;
        _stepActions = new Dictionary<string, Action>()
        {
            {"SpeedUpServe", SpeedUpServeAction},
            {"ResetSpeedUpServe", ResetSpeedUpServeAction},
            {"ForceStaffHire", ForceStaffHireAction},
            {"ResetStaffHire", ResetStaffHireAction},
        };
        _lock = false;
    }

    void Start()
    {
        EventModule.Instance.Register(EventDefine.TutorialStepDone, OnTutorialStepDone);
        _dialogBtn.onClick.AddListener(OnDialogBtn);
        DoStep();
    }
    public void OnDestroy()
    {
        EventModule.Instance?.UnRegister(EventDefine.TutorialStepDone, OnTutorialStepDone);
    }
    private void OnDialogBtn()
    {
        if(!_inStep) return;
        if(_lock) return;
        if(_hasTarget) return;
        if(_hasDialog && _dialogAction != null) {
            _dialogAction.Complete();
            return;
        }
        FinishStep();
    }
    private void OnToggle(bool enable)
    {
        FinishStep();
    }
    public void Reset()
    {
        _dialogObj.SetActive(false);
        _handObj.SetActive(false);
        _holeMask.SetHole(Vector2.zero, 0);
    }
    private async void DoStep()
    {
        Reset();
        if(_tutorialData == null) {
            return;
        }
        var step = _tutorialData.CurStep();
        if(step != null) {
            if(!string.IsNullOrEmpty(step.on_start)) {
                _stepActions[step.on_start].Invoke();
            }

            if(step.enter_delay > 0) {
                await Task.Delay(step.enter_delay);
            }

            if(step.camera_size > 0) {
                RuntimeMgr.Instance.GetWorldCameraCtrl().LockCamera(step.camera_pos, step.camera_size);
                await Task.Delay(300);
                if(gameObject == null) return;
            }

            if(!string.IsNullOrEmpty(step.dialog)) {
                _dialogTxt.text = "";
                _hasDialog = true;
                _dialogObj.SetActive(true);
                _clickTip.SetActive(false);
                var dialogContent = step.dialog.Locale();
                _dialogAction = _dialogTxt.DOText(dialogContent, dialogContent.Length * 0.05f, true).SetEase(Ease.Linear).OnComplete(ShowDialogDone);
            }

            if(!string.IsNullOrEmpty(step.target)) {
                _hasTarget = true;
                GameObject target = GameObject.Find(step.target);
                if(target == null || !target.activeInHierarchy) {
                    Debug.LogError("[TutorailView] Tutorial Target Error!");
                    //TutorialModule.Instance.FinishTutorial();
                    Hide();
                } else {
                    var targetPos = Vector3.zero;
                    Button targetBtn = target.GetComponent<Button>();
                    if(targetBtn != null) {
                        targetPos = target.transform.position;
                        targetBtn.onClick.AddListener(FinishStep);
                    }
                    Toggle targetToggle = target.GetComponent<Toggle>();
                    if(targetToggle != null) {
                        targetPos = target.transform.position;
                        targetToggle.onValueChanged.AddListener(OnToggle);
                    }
                    BuildingCtrl targetCtrl = target.GetComponent<BuildingCtrl>();
                    if(targetCtrl != null) {
                        targetPos = new Vector3(0, 0, transform.position.z);
                        targetCtrl.OnClickTriggerMod += FinishStep;
                    }
                    _handObj.transform.position = targetPos;
                    _handObj.SetActive(true);
                    _holeMask.SetHole(targetPos, step.target_size);
                }
            }
            _inStep = true;
        } else {
            //TutorialModule.Instance.FinishTutorial();
            Hide();
        }
    }
    public async void FinishStep()
    {
        _inStep = false;
        var step = _tutorialData.CurStep();
        if(step.save) TutorialModule.Instance.TutorialDone();
        _hasTarget = false;
        _hasDialog = false;

        if(step.camera_size > 0) RuntimeMgr.Instance.GetWorldCameraCtrl().CancelLock();
        if(!string.IsNullOrEmpty(step.on_end)) _stepActions[step.on_end].Invoke();
        if(!string.IsNullOrEmpty(step.target)) {
            GameObject target = GameObject.Find(step.target);
            Button targetBtn = target.GetComponent<Button>();
            if(targetBtn != null) targetBtn.onClick.RemoveListener(FinishStep);
            Toggle targetToggle = target.GetComponent<Toggle>();
            if(targetToggle != null) targetToggle.onValueChanged.RemoveListener(OnToggle);
            BuildingCtrl targetCtrl = target.GetComponent<BuildingCtrl>();
            if(targetCtrl != null) targetCtrl.OnClickTriggerMod -= FinishStep;
            _handObj.SetActive(false);
        }
        if(step.exit_delay > 0) {
            await Task.Delay(step.exit_delay);
            if(gameObject == null) return;
        }
        ++_tutorialData.step;
        DoStep();
    }

    private void ShowDialogDone()
    {
        _clickTip.SetActive(true);
        _dialogComplete = true;
        _dialogAction = null;
    }
    private void OnTutorialStepDone(Component sender, EventArgs args)
    {
        FinishStep();
    }

    private void SpeedUpServeAction()
    {
        Time.timeScale = 4.0f;
        BuildingModule.Instance.InServeTutorial = true;
        _holeMask.SetTransparent(true);
        _lock = true;
    }
    private void ResetSpeedUpServeAction()
    {
        Time.timeScale = 1.0f;
        BuildingModule.Instance.InServeTutorial = false;
        _holeMask.SetTransparent(false);
        _lock = false;
    }
    private void ForceStaffHireAction()
    {
        StaffModule.Instance.ForceMakeOffer = true;
    }
    private void ResetStaffHireAction()
    {
        StaffModule.Instance.ForceMakeOffer = false;
    }
}
