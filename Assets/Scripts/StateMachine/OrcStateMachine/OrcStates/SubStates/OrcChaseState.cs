using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcChaseState : OrcCombatState
{

    private float movementUpdateTimer = 0f;

    public OrcChaseState(Orc orc, OrcStateMachine stateMachine, OrcData orcData, string animBoolName) : base(orc, stateMachine, orcData, animBoolName) {
    }

    public override void DoChecks() {
        base.DoChecks();
    }

    public override void Enter() {
        base.Enter();
        orc.MovementController.speed = orcData.chaseSpeed;

        orc.MovementController.GetMoveCommand(orc.targetGO.transform.position);
    }

    public override void Exit() {
        base.Exit();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        if (orc.waypointActive) {
            stateMachine.ChangeState(orc.WaypointState);
            return;
        }

        movementUpdateTimer += Time.deltaTime;
        if (movementUpdateTimer >= orcData.movementUpdateTime) {
            movementUpdateTimer = 0f;
            orc.MovementController.GetMoveCommand(orc.targetGO.transform.position);
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
}
