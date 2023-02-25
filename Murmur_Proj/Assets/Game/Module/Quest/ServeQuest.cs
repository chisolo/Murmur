using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServeQuest : QuestData
{
    public override bool IsCompleted()
    {
        return archive.progress >= config.target;
    }
    public override void OnAttach()
    {
        
    }
    public override bool OnServe(string service)
    {
        if(archive.status > 0) return false;
        if(config.param != service) return false;
        archive.progress++;
        if(IsCompleted()) {
            archive.status = 1;
        }
        return true;
    }
}
