using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class TalentListView : MonoBehaviour
{
    [SerializeField]
    private RectTransform _content;

    [SerializeField]
    private TalentListItem _listItemPrefab;

    [SerializeField]
    private TalentSectionTitle _sectionTitlePrefab;

    [SerializeField]
    private LayoutElement _listRowPrefab;

    [SerializeField]
    private GameObject _reddit;
    [SerializeField]
    private Text _redditText;


    private List<TalentListItem> _goList = new List<TalentListItem>();
    private string _tabName;
    private int _lastNum = 0;
    private long _timerId;

    public void Init(List<TalentData> talentList)
    {
        var lastSection = "";
        LayoutElement lastRow = null;

        foreach (var talentData in talentList) {
            _tabName = talentData.Cfg.tab;
            if (talentData.Cfg.section != lastSection) {
                if (!string.IsNullOrEmpty(lastSection)) {
                    var row = Instantiate(_listRowPrefab, _content);
                    row.preferredHeight = 25;
                }
                lastSection = talentData.Cfg.section;
                AddSection(lastSection);
                lastRow = null;
            }

            if (lastRow == null || lastRow.transform.childCount == 4) {
                lastRow = Instantiate(_listRowPrefab, _content);
            }
            var item = Instantiate(_listItemPrefab, lastRow.transform);
            item.Init(talentData);
            _goList.Add(item);
        }

        var bottom = Instantiate(_listRowPrefab, _content);
        bottom.preferredHeight = 25;
        _reddit.SetActive(false);
        _timerId = TimerModule.Instance.CreateFrameTimer(0.1f, OnTimerUpdate, loop:-1);
    }

    private void OnDestroy()
    {
        TimerModule.Instance?.CancelTimer(_timerId);
    }

    private void AddSection(string section)
    {
        var sectionTitle = Instantiate(_sectionTitlePrefab, _content);
        sectionTitle.Init(section);
    }

    void OnTimerUpdate()
    {
        if (this == null || gameObject == null) {
            TimerModule.Instance?.CancelTimer(_timerId);
            return;
        }
        if (gameObject.activeSelf) {
            foreach (var item in _goList) {
                item.UpdateStatus();
            }
        }

        var num = 0;
        foreach (var data in _goList) {
            if (data.Status == TalentStatus.CanResearch) {
                num++;
            }
        }
        num = Mathf.Min(num, 9);

        if (num != _lastNum) {
            if (num > 0) {
                _reddit.gameObject.SetActive(true);
                _redditText.text = string.Format("+{0}", num);
            } else {
                _reddit.gameObject.SetActive(false);
            }
            _lastNum = num;
        }
    }

    public void GotoItem(string talentId)
    {
        foreach (var item in _goList) {
            if (item.TalentId == talentId) {
                var row = item.transform.parent as RectTransform;
                var pos = -(row.anchoredPosition.y + 0.5f * row.rect.height);
                pos -= 200;

                _content.anchoredPosition = new Vector2(_content.anchoredPosition.x, pos);
                break;
            }
        }
    }
}