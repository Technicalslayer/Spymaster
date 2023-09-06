using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        orc.MovementController.speed = orcData.chaseSpeed;
        if(orc.waypoint == null) {
            stateMachine.ChangeState(orc.SeekHouseState);
            return;
        }
        orc.MovementController.GetMoveCommand(orc.waypoint.transform.position);
    }

    public override void Exit() {
        base.Exit();
        orc.waypointActive = false;
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        if(orc.waypoint == null || orc.waypoint.IsDestroyed()) {
            stateMachine.ChangeState(orc.IdleState); return; //in case waypoint is destroyed before reaching it
        }

        if(Vector2.Distance(orc.waypoint.transform.position, orc.transform.position) < 1f) {
            stateMachine.ChangeState(orc.IdleState);
        }

        if(Time.time - startTime >= 10f) {
            //just in case
            stateMachine.ChangeState(orc.IdleState);
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
}
