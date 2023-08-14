using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcWaypointState : OrcState
{
    public OrcWaypointState(Orc orc, OrcStateMachine stateMachine, OrcData orcData, string animBoolName) : base(orc, stateMachine, orcData, animBoolName) {
    }

    public override void DoChecks() {
        base.DoChecks();
    }

    public override void Enter() {
        base.Enter();
    }

    public override void Exit() {
        base.Exit();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
}
