using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PuppetAction
{
    public PuppetCtrl owner;
    public bool finish;
    public Action<PuppetCtrl> callback;
    public virtual void Execute(PuppetCtrl puppetCtrl) 
    {
        owner = puppetCtrl;
        finish = false;
    }
    public virtual void Tick()
    {

    }
    public virtual void Finish()
    {

    }
    public virtual void Cancel()
    {
        owner = null;
        finish = false;
        callback = null;
    }
}
