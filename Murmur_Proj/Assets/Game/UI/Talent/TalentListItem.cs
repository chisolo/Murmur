using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class TalentListItem : MonoBehaviour
{
    private readonly Color _completeNameColor = new Color(236 / 255f, 234/ 255f, 211/ 255f);
    private readonly Color _notStartColor = new Color(73/255f, 59/255f, 44/255f);

    [SerializeField]
    private Image _icon;
    [SerializeField]
    private Image _miniIcon;
    [SerializeField]
    private GameObject _lvUp;
    [SerializeField]
    private GameObject _completeView;
    [SerializeField]
    private GameObject _notStartView;
    [SerializeField]
    private GameObject _notStartViewCover;
    [SerializeField]
    private Text _nameText;
    [SerializeField]
    private GameObject _timeView;
    [SerializeField]
    private TimeProgressBar _timeProgress;


    private TalentData _talentData;
    private TalentStatus _talentStatus;
    public TalentStatus Status => _talentData.Status;
    public string TalentId => _talentData.Id;

    public void Init(TalentData talentData)
    {
        _talentData = talentData;
        _completeView.SetActive(false);
        _notStartView.SetActive(false);
        _timeProgress.gameObject.SetActive(false);
        _timeView.SetActive(false);
        _lvUp.SetActive(false);
        _notStartViewCover.SetActive(false);

        _icon.ShowSprite(AtlasDefine.GetTalentIconPath(talentData.Cfg.icon));
        if (!string.IsNullOrEmpty(talentData.Cfg.mini_icon)) {
            _miniIcon.ShowSprite(AtlasDefine.GetTalentIconPath(talentData.Cfg.mini_icon));
        } else {
            _miniIcon.gameObject.SetActive(false);
        }

        _timeProgress.Init(_talentData, null, () => {
            _timeProgress.gameObject.SetActive(false);
            _timeView.SetActive(false);
        });

        _nameText.text = talentData.Cfg.name.Locale();

        _talentStatus = _talentData.Status;

        if (_talentStatus == TalentStatus.Complete) {
            // 研究完成
            _completeView.SetActive(true);
            _nameText.color = _completeNameColor;
        } else if (_talentStatus == TalentStatus.InProgress) {
            // 研究中
            _completeView.SetActive(true);
            _timeProgress.gameObject.SetActive(true);
            _timeView.SetActive(true);
            _nameText.color = _completeNameColor;

        } else {
            // 不可研究
            _notStartView.SetActive(true);
            _notStartViewCover.SetActive(true);
            _nameText.color = _notStartColor;

            if (_talentStatus == TalentStatus.CanResearch) {
                // 可研究
                _lvUp.SetActive(true);
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

        if (_talentStatus == TalentStatus.InProgress) {
            // 研究中
            _lvUp.SetActive(false);
            _notStartView.SetActive(false);
            _notStartViewCover.SetActive(false);
            _completeView.SetActive(true);
            _timeProgress.gameObject.SetActive(true);
            _timeView.SetActive(true);
            _nameText.color = _completeNameColor;

        } else if (_talentStatus == TalentStatus.CanResearch) {
            _lvUp.SetActive(true);
        } else {
            // 不可研究
            _lvUp.SetActive(false);
        }
    }

    public void OnClickSelfBtn()
    {
        var arg = new TalentItemDetail.UIArgs();
        arg.talentId = _talentData.Id;
        UIMgr.Instance.OpenUIByClick(TalentItemDetail.PrefabPath, arg, false, false);
    }

}