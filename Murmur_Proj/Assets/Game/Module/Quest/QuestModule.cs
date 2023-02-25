using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Lemegeton;

public class QuestModule : Singleton<QuestModule>
{
    QuestModule() {}

    private List<QuestConfig> _questConfigs;
    private QuestArchive _questArchive;
    private QuestData[] _quests;
    private bool _inited = false;
    private bool _dirty = false;
    public void Init()
    {
        if(_inited) return;
        _questConfigs = ConfigModule.Instance.Quests();
        _questArchive = ArchiveModule.Instance.GetArchive<QuestArchive>();
        _quests = new QuestData[GameUtil.QuestCount];

        if(_questArchive.next == 0) {
            for(int i = 0; i < GameUtil.QuestCount; ++i) {
                var questConfig = _questConfigs[i];
                var archiveData = new QuestArchiveData(questConfig.id, 0, 0);
                _questArchive.quests.Add(archiveData);
            }
            _questArchive.next = GameUtil.QuestCount;
            Save();
        }
        for(int i = 0; i < _questArchive.quests.Count; ++i) {
            var questConfig = _questConfigs.Find(e => e.id == _questArchive.quests[i].id);
            var quest = CreateQuest(questConfig);
            quest.Init(questConfig, _questArchive.quests[i], i);
            _quests[i] = quest;
            quest.OnAttach();
        }

        if(_quests.Length > 0) TimerModule.Instance.CreateTimer(1f, OnTick, false, null, -1);
        _inited = true;
    }

    private QuestData CreateQuest(QuestConfig config)
    {
        var questType = config.type;
        switch (questType)
        {
            case QuestType.Build:
            {
                return new BuildQuest();
            }
            case QuestType.Hire:
            {
                return new HireQuest();
            }
            case QuestType.Serve:
            {
                return new ServeQuest();
            }
            case QuestType.Produce:
            {
                return new ProduceQuest();
            }
            case QuestType.Deliver:
            {
                return new DeliverQuest();
            }
        }
        return null;
    }

    private void AssignQuest(int index)
    {
        var next = _questArchive.next;
        if(_questArchive.next >= _questConfigs.Count) return;
        var questConfig = _questConfigs[next];
        var nextArchive = new QuestArchiveData(questConfig.id, 0, 0);
        _questArchive.SetData(nextArchive, index);
        var quest = CreateQuest(questConfig);
        quest.Init(_questConfigs[next], nextArchive, index);
        _quests[index] = quest;
        quest.OnAttach();
        _questArchive.next++;
        Save();
        using (UpdateQuestArgs args = UpdateQuestArgs.Get()) {
            args.questId = quest.config.id;
            args.index = index;
            EventModule.Instance.FireEvent(EventDefine.UpdateQuest, args);
        }
    }
    public void OnServe(string service) 
    {
        for(int i = 0; i < _quests.Length; ++i) {
            if(_quests[i].OnServe(service)) {
                _dirty = true;
                using (UpdateQuestArgs args = UpdateQuestArgs.Get()) {
                    args.questId = string.Empty;
                    args.index = i;
                    EventModule.Instance.FireEvent(EventDefine.UpdateQuest, args);
                }
            };
        }
        if(_dirty) Save();
    }
    public void OnProduce(string ingredient) 
    {
        for(int i = 0; i < _quests.Length; ++i) {
            if(_quests[i].OnProduce(ingredient)) {
                _dirty = true;
                using (UpdateQuestArgs args = UpdateQuestArgs.Get()) {
                    args.questId = string.Empty;
                    args.index = i;
                    EventModule.Instance.FireEvent(EventDefine.UpdateQuest, args);
                }
            }
        }
        if(_dirty) Save();
    }
    public void OnDeliver(string ingredient, int count) 
    { 
        for(int i = 0; i < _quests.Length; ++i) {
            if(_quests[i].OnDeliver(ingredient, count)) {
                _dirty = true;
                using (UpdateQuestArgs args = UpdateQuestArgs.Get()) {
                    args.questId = string.Empty;
                    args.index = i;
                    EventModule.Instance.FireEvent(EventDefine.UpdateQuest, args);
                }
            }
        }
        if(_dirty) Save();
    }

    private void OnTick()
    {
        if(!_inited) return;
        for(int i = 0; i < _quests.Length; ++i) {
            if(_quests[i].OnTick()) {
                _dirty = true;
                using (UpdateQuestArgs args = UpdateQuestArgs.Get()) {
                    args.questId = string.Empty;
                    args.index = i;
                    EventModule.Instance.FireEvent(EventDefine.UpdateQuest, args);
                }
            }
        }
        if(_dirty) Save();
    }
    private void Save()
    {
        ArchiveModule.Instance.SaveArchive(_questArchive);
        _dirty = false;
    }
    public QuestData GetQuestData(int index)
    {
        return _quests[index];
    }
    public void Claim(int index)
    {
        var claimQuest = _quests[index];
        if(claimQuest.IsCompleted()) {
            AddQuestReward(claimQuest.config.reward_type, claimQuest.config.reward_amount);
            TraceModule.Instance.TraceQuest(claimQuest.config.id);
            AssignQuest(index);
        }
    }
    public bool HasCompletedQuest()
    {
        var ret = false;
        foreach(var quest in _quests) {
            ret |= quest.archive.status > 0;
        }
        return ret;
    }
    private void AddQuestReward(string itemType, int number)
    {
        if(itemType == ItemType.Money) {
            PlayerModule.Instance.AddMoneyWithBigEffect(number, true);
        } else if(itemType == ItemType.Coin) {
            PlayerModule.Instance.AddCoinWithBigEffect(number, true);
        }
    }
}
