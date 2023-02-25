using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;

public class QuestView : PopupUIBaseCtrl
{
    public static string PrefabPath = "Assets/Res/UI/Prefab/Quest/QuestView.prefab";

    [SerializeField] Transform _questContent;
    [SerializeField] GameObject _itemPrefab;
    
    private List<QuestItem> _questItems;

    public override void Init(PopupUIArgs arg)
    {
        EventModule.Instance.Register(EventDefine.UpdateQuest, OnUpdateQuestEvent);
    }
    void OnDestroy()
    {
        EventModule.Instance?.UnRegister(EventDefine.UpdateQuest, OnUpdateQuestEvent);
    }
    void Start()
    {
        _questItems = new List<QuestItem>();
        Refresh();
    }
    void Refresh()
    {
        for(int i = 0; i < GameUtil.QuestCount; ++i) {
            QuestItem item = GameObject.Instantiate(_itemPrefab, _questContent).GetComponent<QuestItem>();
            item.Init(i);
            _questItems.Add(item);
        }
    }
    void RefreshItem(int index)
    {
        _questItems[index].Refresh();
    }
    IEnumerator ResetItem(int index)
    {
        yield return new WaitForSeconds(0.5f);
        _questItems[index].Reset(index);
    }
    private void OnUpdateQuestEvent(Component sender, EventArgs e)
    {
        var arg = e as UpdateQuestArgs;
        if(arg.questId != String.Empty) {
            StartCoroutine(ResetItem(arg.index));
        } else {
            RefreshItem(arg.index);
        }
    }
}
