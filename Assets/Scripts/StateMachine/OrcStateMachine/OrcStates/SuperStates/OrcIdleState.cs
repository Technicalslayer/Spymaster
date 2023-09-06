using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcIdleState : OrcState
{
    private float walkTime = 1f; //if in idle state this long, then pick a new move command
    private float walkTimer = 0f;

    public OrcIdleState(Orc orc, OrcStateMachine stateMachine, OrcData orcData, string animBoolName) : base(orc, stateMachine, orcData, animBoolName) {
    }

    public override void DoChecks() {
        base.DoChecks();
    }

    public override void Enter() {
        base.Enter();

        orc.MovementController.speed = orcData.idleSpeed;
        //pick random direction to move in
        orc.MovementController.GetMoveCommand(Random.insideUnitCircle * 5f);
    }

    public override void Exit() {
        base.Exit();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();
        walkTimer += Time.deltaTime;

        if(Time.time - startTime > orcData.idleTime) {
            stateMachine.ChangeState(orc.SeekHouseState);
        }
        else if(walkTimer >= walkTime) {
            orc.MovementController.GetMoveCommand(Random.insideUnitCircle * 5f);
            walkTime = Random.Range(1f, 3f); //pick random amount of time to go foward
            walkTimer = 0f; //reset timer
        }

        if (orc.waypointActive) {
            stateMachine.ChangeState(orc.WaypointState);
            return;
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
}
