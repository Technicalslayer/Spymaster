using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcAttackHouseState : OrcState
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
    }

    public override void Exit() {
        base.Exit();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();
        
        if(attackTimer > orcData.attackTime) {
            attackTimer = 0;
            //play sound and anim
            orc.attackSound.Play();
            house.TakeDamage();
            //play anim, but have a transition set up to automatically go back to the "idle" stance
            orc.Anim.SetBool("attack", true);
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
