using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;

public class TalentButton : MonoBehaviour
{

    [SerializeField]
    private Flash _flash;
    [SerializeField]
    private Text _numText;
    [SerializeField]
    private GameObject _numRoot;
    [SerializeField]
    private GameObject _progressRoot;
    [SerializeField]
    private Image _progressBar;

    private int _lastNum = 0;

    private TalentData _talentData = null;

    private long _timerId;

    private void Start()
    {
        _flash.gameObject.SetActiveIfNeed(false);
        _numRoot.gameObject.SetActive(false);
        _progressRoot.SetActive(false);

        FindShowingTalent();

        _timerId = TimerModule.Instance.CreateTimer(1, OnUpdate, loop:-1);

        EventModule.Instance.Register(EventDefine.TalentResearchStart, OnTalentStartEvent);
        EventModule.Instance.Register(EventDefine.TalentResearchComplete, OnTalentCompleteEvent);
    }

    private void OnDestroy()
    {
        TimerModule.Instance?.CancelTimer(_timerId);
        EventModule.Instance?.UnRegister(EventDefine.TalentResearchStart, OnTalentStartEvent);
        EventModule.Instance?.UnRegister(EventDefine.TalentResearchComplete, OnTalentCompleteEvent);
    }

    private void FindShowingTalent()
    {
        var max = long.MaxValue;
        var id = "";
        foreach (var team in TalentModule.Instance.WorkTeam.teams) {
            if (!string.IsNullOrEmpty(team.owner) && team.endTime > 0 && team.endTime < max) {
                max = team.endTime;
                id = team.owner;
            }
        }

        if (!string.IsNullOrEmpty(id)) {
            _talentData = TalentModule.Instance.GetTalentData(id);

            var remain = _talentData.EndTime - NtpModule.Instance.UtcNowSeconds;
            _progressBar.fillAmount = 1 - (float)remain / _talentData.Duration;
            _progressRoot.SetActive(true);
        }
    }

    public void OnClickTalentBtn()
    {
        Resources.UnloadUnusedAssets();
        var arg = new TalentUICtrl.Args();
        UIMgr.Instance.OpenUIByClick(TalentUICtrl.PrefabPath, arg, false, true);
    }

    void OnUpdate()
    {
        var list = TalentModule.Instance.GetTalentDatas();

        var num = 0;
        foreach (var data in list) {
            if (data.Status == TalentStatus.CanResearch) {
                num++;
            }
        }
        var show = num;
        var hasEmpty = TalentModule.Instance.WorkTeam.HasEmpty();
        num = Mathf.Min(num, 9);

        if (hasEmpty) {
            if (num != _lastNum) {
                if (num > 0) {
                    if (show > 9) {
                        _numText.text = string.Format("+{0}", num);
                    } else {
                        _numText.text = string.Format("{0}", num);
                    }
                }
                _lastNum = num;
            }

            _flash.gameObject.SetActiveIfNeed(num != 0);
            _numRoot.gameObject.SetActiveIfNeed(num != 0);
        } else {
            _flash.gameObject.SetActiveIfNeed(false);
            _numRoot.gameObject.SetActiveIfNeed(false);
        }

        if (_talentData != null && _talentData.EndTime > 0) {
            var remain = _talentData.EndTime - NtpModule.Instance.UtcNowSeconds;
            _progressBar.fillAmount = 1 - (float)remain / _talentData.Duration;
        }
    }

    private void OnTalentStartEvent(Component sender, System.EventArgs e)
    {
        var arg = e as TalentResearchEventArgs;

        FindShowingTalent();
    }

    private void OnTalentCompleteEvent(Component sender, System.EventArgs e)
    {
        var arg = e as TalentResearchEventArgs;
        if (_talentData != null && arg.talentId == _talentData.Id) {
            _progressRoot.SetActive(false);
        }

        FindShowingTalent();
    }
}
