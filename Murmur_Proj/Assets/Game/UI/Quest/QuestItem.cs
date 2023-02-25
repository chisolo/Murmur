using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;

public class QuestItem : MonoBehaviour
{
    [SerializeField] GameObject _completedBg;
    [SerializeField] GameObject _normalBg;
    [SerializeField] GameObject _check;
    [SerializeField] Text _descTxt;
    [SerializeField] ToggleGo _bonusTg;
    [SerializeField] Text _bonusAmount;
    [SerializeField] Text _targetTxt;
    [SerializeField] Button _claimBtn;
    [SerializeField] Button _solveBtn;
    [SerializeField] GameObject _maxCheck;

    private QuestData _quest;
    private int _index;
    public void Init(int index)
    {
        _quest = QuestModule.Instance.GetQuestData(index);
        _index = index;
    }
    void Start()
    {
        _claimBtn.onClick.AddListener(OnClaim);
        _solveBtn.onClick.AddListener(OnSolve);
        Refresh();
    }
    public void Reset(int index)
    {
        _quest = QuestModule.Instance.GetQuestData(index);
        _index = index;
        Refresh();
    }
    public void Refresh()
    {
        if(_quest.IsCompleted()) {
            _completedBg.SetActive(true);
            _normalBg.SetActive(false);
            _check.SetActive(true);
        } else {
            _completedBg.SetActive(false);
            _normalBg.SetActive(true);
            _check.SetActive(false);
        }
        _descTxt.text = _quest.config.desc.Locale();
        if(_quest.config.reward_type == ItemType.Money) {
            _bonusTg.Toggle(0);
        } else {
            _bonusTg.Toggle(1);
        }
        _bonusAmount.text = "+" + _quest.config.reward_amount;

        var questStatus = _quest.archive.status;
        if(questStatus == 0) {
            _completedBg.SetActive(false);
            _normalBg.SetActive(true);
            _check.SetActive(false);
            _targetTxt.gameObject.SetActive(true);
            _maxCheck.gameObject.SetActive(false);
            _claimBtn.gameObject.SetActive(false);
            _solveBtn.gameObject.SetActive(_quest.config.type == QuestType.Build);
            _targetTxt.text = string.Format("<color=#DF762D>{0}</color>/{1}", _quest.archive.progress, _quest.config.target);
        } else if(questStatus == 1) {
            _completedBg.SetActive(true);
            _normalBg.SetActive(false);
            _check.SetActive(true);
            _targetTxt.gameObject.SetActive(false);
            _claimBtn.gameObject.SetActive(true);
            _maxCheck.gameObject.SetActive(false);
            _solveBtn.gameObject.SetActive(false);
        } else {
            _completedBg.SetActive(true);
            _normalBg.SetActive(false);
            _check.SetActive(true);
            _targetTxt.gameObject.SetActive(false);
            _claimBtn.gameObject.SetActive(false);
            _maxCheck.gameObject.SetActive(true);
            _solveBtn.gameObject.SetActive(false);
        }
    }
    private void OnClaim()
    {
        QuestModule.Instance.Claim(_index);
        _targetTxt.gameObject.SetActive(false);
        _claimBtn.gameObject.SetActive(false);
        _maxCheck.gameObject.SetActive(true);
    }

    private void OnSolve()
    {
        if(_quest.config.type == QuestType.Build) {
            BuildingMgr.Instance.PopupBuildingView(_quest.config.param, true);
        }
    }
}
