using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestData
{
    public QuestArchiveData archive;
    public QuestConfig config;
    public int index;

    public void Init(QuestConfig questConfig, QuestArchiveData questArchive, int idx)
    {
        this.config = questConfig;
        this.archive = questArchive;
        index = idx;
    }

    public virtual bool IsCompleted() { return false; } // 是否完成
    public virtual void OnAttach() {} // 开始
    public virtual bool OnServe(string service) { return false; }
    public virtual bool OnProduce(string ingredient) { return false; }
    public virtual bool OnDeliver(string ingredient, int count) { return false; }
    public virtual bool OnTick() { return false; }

}
