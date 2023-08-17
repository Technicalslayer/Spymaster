using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcSeekHouseState : OrcState
{
    public OrcSeekHouseState(Orc orc, OrcStateMachine stateMachine, OrcData orcData, string animBoolName) : base(orc, stateMachine, orcData, animBoolName) {
    }

    public override void DoChecks() {
        base.DoChecks();
    }

    public override void Enter() {
        base.Enter();
        //update list of potential houses
        orc.houses = orc.FindValidHouses();
        if(orc.houses.Length == 0) {
            stateMachine.ChangeState(orc.IdleState); //just to prevent errors with house checking while changing scenes. May not be necessary
            return;
        }

        //pick random house if no houses within a certain range?
        //first index should be closest house
        if (Vector2.Distance(orc.houses[0].transform.position, orc.transform.position) < orcData.nonRandomSeekDistance) {
            orc.targetGO = orc.houses[0];
            Debug.Log("Within Range");
        }
        else {
            orc.targetGO = orc.houses[Random.Range(0, orc.houses.Length)];
        }

        orc.MovementController.speed = orcData.pillageSpeed;

        orc.MovementController.GetMoveCommand(orc.targetGO.transform.position);
    }

    public override void Exit() {
        base.Exit();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();
        //if at house, enter attack house state
        if(Vector2.Distance(orc.transform.position, orc.targetGO.transform.position) < 2f) {
            stateMachine.ChangeState(orc.AttackHouseState);
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }


}
