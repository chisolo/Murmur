using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HireQuest : QuestData
{
    public override bool IsCompleted()
    {
        return StaffModule.Instance.GetStaffCount() >= config.target;
    }
    public override void OnAttach()
    {
        
    }
    public override bool OnTick()
    {
        if(archive.status <= 0 && IsCompleted()) {
            archive.status = 1;
            return true;
        }
        return false;
    }
}
