using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class TalentItemDetail : PopupUIBaseCtrl
{
    public class UIArgs : PopupUIArgs
    {
        public string talentId;
    }

    public static string PrefabPath = "Assets/Res/UI/Prefab/Talent/TalentItemDetail.prefab";

    [SerializeField]
    private Text _nameText;
    [SerializeField]
    private Text _descText;

    [SerializeField]
    private Image _icon;
    [SerializeField]
    private Image _miniIcon;

    [SerializeField]
    private Text _timeText;
    [SerializeField]
    private Text _costText;

    [SerializeField]
    private Button _improveBtn;

    [SerializeField]
    private GameObject _completeView;
    [SerializeField]
    private GameObject _improveView;

    [SerializeField]
    private GameObject _requireView;
    [SerializeField]
    private Text _requireNameText;

    [SerializeField]
    private AdTimeProgress _inProgressView;

    [SerializeField]
    private Text _devTeamCount;

    private TalentData _talentData;

    private TalentStatus _talentStatus;

    public override void Init(PopupUIArgs arg)
    {
        var param = (UIArgs)arg;

        var data = TalentModule.Instance.GetTalentData(param.talentId);

        Init(data);
    }

    public void Init(TalentData talentData)
    {
        _talentData = talentData;
        _completeView.SetActive(false);
        _improveView.SetActive(false);
        _requireView.SetActive(false);
        _inProgressView.gameObject.SetActive(false);

        _icon.ShowSprite(AtlasDefine.GetTalentIconPath(talentData.Cfg.icon));
        if (!string.IsNullOrEmpty(talentData.Cfg.mini_icon)) {
            _miniIcon.ShowSprite(AtlasDefine.GetTalentIconPath(talentData.Cfg.mini_icon));
        } else {
            _miniIcon.gameObject.SetActive(false);
        }

        _nameText.text = talentData.Cfg.name.Locale();
        _descText.text = talentData.Cfg.desc.Locale();
        _inProgressView.Init(_talentData);

        _talentStatus = _talentData.Status;
        if (_talentStatus == TalentStatus.Complete) {
            // 研究完成
            _completeView.SetActive(true);
        } else if (_talentStatus == TalentStatus.InProgress) {
            // 研究中
            _inProgressView.gameObject.SetActive(true);
        } else {
            // 可研究
            _improveView.SetActive(true);
            _improveBtn.interactable = true;
            _costText.text = FormatUtil.Currency(talentData.Cost);
            _timeText.text = FormatUtil.FormatTimeAuto(talentData.Cfg.time);

            if (_talentStatus == TalentStatus.CannotResearch) {
                // 不可研究
                _improveBtn.interactable = false;

                if (!string.IsNullOrEmpty(talentData.Cfg.require_id) && !TalentModule.Instance.IsComplete(talentData.Cfg.require_id)) {
                    _requireView.SetActive(true);
                    var requireData = TalentModule.Instance.GetTalentData(talentData.Cfg.require_id);
                    _requireNameText.text = LocaleModule.Instance.GetLocale("TALENT_REQUIRE", requireData.Cfg.name.Locale());
                    //_requireNameText.text = string.Format("{0} <b>{1}</b>", "TALENT_REQUIRE".Locale(), requireData.Cfg.name.Locale());
                }
            }
        }
    }

    public void UpdateStatus()
    {
        if (_talentStatus == TalentStatus.Complete) {
            return;
        }

        var lastStatus = _talentStatus;
        _talentStatus = _talentData.Status;
        if (lastStatus == _talentStatus) {
            return;
        }

        if (_talentStatus == TalentStatus.Complete) {
            _completeView.SetActive(true);
            _inProgressView.gameObject.SetActive(false);
        } else if (_talentStatus == TalentStatus.InProgress) {
            // 研究中
            _inProgressView.gameObject.SetActive(true);
            _inProgressView.UpdateStatus();
            _improveView.SetActive(false);

        } else if (_talentStatus == TalentStatus.CanResearch) {
            // 可研究
            _improveView.SetActive(true);
            _improveBtn.interactable = true;
            _requireView.SetActive(false);
        } else {
            // 不可研究
            _improveBtn.interactable = false;
        }
    }

    void Update()
    {
        UpdateStatus();
    }

    public void OnClickImproveBtn()
    {
        if (TalentModule.Instance.CanResearch(_talentData)) {
            if (TalentModule.Instance.WorkTeam.HasEmpty()) {
                TalentModule.Instance.StartResearch(_talentData);
                UpdateStatus();
            } else {
                var arg = new TeamBusyPopupCtrl.Args();
                arg.title = GameUtil.TalentTeamFullTitleText.Locale();
                arg.body = GameUtil.TalentTeamFullBodyText.Locale();
                arg.shopText = GameUtil.TalentTeamFullShopText.Locale();
                arg.teamMax = TalentModule.Instance.WorkTeam.IsTeamMax();
                arg.timeData = TalentModule.Instance.GetProcessTalentData();
                UIMgr.Instance.OpenUIByClick(TeamBusyPopupCtrl.PrefabPath, arg, false, false);
            }
        }

    }

    public void OnClickRequiredBtn()
    {
        if (!string.IsNullOrEmpty(_talentData.Cfg.require_id)) {
            var requireData = TalentModule.Instance.GetTalentData(_talentData.Cfg.require_id);

            _inProgressView.Reset();
            this.Init(requireData);

            using (var arg  = TalentOpenEventArgs.Get()) {
                arg.talentId = requireData.Id;
                EventModule.Instance.FireEvent(EventDefine.TalentOpen, arg);
            }
        }
    }

    public void ReduceTime(long time)
    {
        TalentModule.Instance.ReduceTime(_talentData, time);
    }

    public void Finish()
    {
        TalentModule.Instance.CompleteResearch(_talentData);
    }

}