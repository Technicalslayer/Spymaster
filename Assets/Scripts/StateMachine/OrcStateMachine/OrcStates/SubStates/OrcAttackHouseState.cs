using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcAttackHouseState : OrcPillageState
{
    private float attackTimer;
    private HouseController house;

    public OrcAttackHouseState(Orc orc, OrcStateMachine stateMachine, OrcData orcData, string animBoolName) : base(orc, stateMachine, orcData, animBoolName) {
    }

    public override void DoChecks() {
        base.DoChecks();
    }

    public override void Enter() {
        base.Enter();
        house = orc.targetGO.GetComponent<HouseController>();
        attackTimer = orcData.attackTime; //attack right away the first time
        orc.MovementController.enabled = false;
    }

    public override void Exit() {
        base.Exit();
        orc.MovementController.enabled = true;
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        attackTimer += Time.deltaTime;
        if(attackTimer > orcData.attackTime) {
            attackTimer = 0;
            //play sound and anim
            orc.attackSound.Play();
            house.TakeDamage();
            //play anim, but have a transition set up to automatically go back to the "idle" stance
            orc.Anim.SetTrigger("attack");
            if(house.health <= 0) {
                //house is done for, exit state
                stateMachine.ChangeState(orc.SeekHouseState);
            }
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
}
