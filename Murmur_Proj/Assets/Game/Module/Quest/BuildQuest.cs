using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lemegeton;
public class BuildQuest : QuestData
{
    public override bool IsCompleted()
    {
        return BuildingModule.Instance.GetBuilding(config.param).GetLevel() >= config.target;
    }
    public override void OnAttach()
    {
    }
    public override bool OnTick()
    {
        if(archive.status <= 0 &&　IsCompleted()) {
            archive.status = 1;
            return true;
        }
        return false;
    }
}
