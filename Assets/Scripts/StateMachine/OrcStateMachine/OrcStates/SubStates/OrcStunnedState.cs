using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcStunnedState : OrcState
{
    public OrcStunnedState(Orc orc, OrcStateMachine stateMachine, OrcData orcData, string animBoolName) : base(orc, stateMachine, orcData, animBoolName) {
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
        if (Time.time - startTime >= orcData.stunTime) {
            stateMachine.ChangeState(orc.ChaseState); //chase until death
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
        orc.RB.velocity = Vector2.Lerp(orc.RB.velocity, Vector2.zero, (Time.time - startTime) / orcData.stunTime);
    }
}
