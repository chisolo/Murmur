using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateQuestArgs : BaseEventArgs<UpdateQuestArgs>
{
    public string questId;
    public int index;
}