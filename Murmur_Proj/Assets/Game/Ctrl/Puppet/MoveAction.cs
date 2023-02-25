using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using Lemegeton;

public class MoveAction : PuppetAction
{
    private SpotConfig _spot;
    private float _distance;
    private NavMeshAgent _navAgent;
    public static MoveAction Get(PuppetCtrl owner, SpotConfig spotConfig, Action<PuppetCtrl> callback, float distance)
    {
        MoveAction action = GenericPool<MoveAction>.Get();
        action.owner = owner;
        action.finish = false;
        action.callback = callback;
        action._spot = spotConfig;
        action._distance = distance;
        return action;
    }
    public override void Execute(PuppetCtrl puppetCtrl) 
    {
        base.Execute(puppetCtrl);
        _navAgent = owner.NavAgent;
        _navAgent.ResetPath();
        _navAgent.SetDestination(_spot.position);
        _navAgent.updateRotation = true;
    }
    public override void Tick()
    {
        owner.Animator.SetFloat(ActionDefine.Speed, owner.NavAgent.velocity.magnitude);
        if(_navAgent == null) return;
        if(!Moving()) {
            if(_spot.strict)  owner.transform.rotation = _spot.rotation;
            finish = true;
        }
        if(callback != null && !_navAgent.pathPending && _navAgent.remainingDistance <= _distance) {
            callback.Invoke(owner);
            callback = null;
        }
    }
    public override void Finish()
    {
        Cancel();
    }
    public override void Cancel()
    {
        owner.Animator.SetFloat(ActionDefine.Speed, 0);
        _navAgent.ResetPath();
        base.Cancel();
        GenericPool<MoveAction>.Release(this);
    }

    private bool Moving()
    {
        if(_navAgent == null) return false;
        return _navAgent.pathPending || _navAgent.remainingDistance > _navAgent.stoppingDistance || _navAgent.velocity != Vector3.zero;
    }
}
