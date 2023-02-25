using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverQuest : QuestData
{
    public override bool IsCompleted()
    {
        return archive.progress >= config.target;
    }
    public override void OnAttach()
    {
        
    }
    public override bool OnDeliver(string ingredient, int count)
    {
        if(archive.status > 0) return false;
        if(config.param != ingredient) return false;
        archive.progress += count;
        if(IsCompleted()) {
            archive.status = 1;
        }
        return true;
    }

}
