using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProduceQuest : QuestData
{
    public override bool IsCompleted()
    {
        return archive.progress >= config.target;
    }
    public override void OnAttach()
    {
        
    }
    public override bool OnProduce(string ingredient)
    {
        if(archive.status > 0) return false;
        if(config.param != ingredient) return false;
        archive.progress++;
        if(IsCompleted()) {
            archive.status = 1;
        }
        return true;
    }
}
