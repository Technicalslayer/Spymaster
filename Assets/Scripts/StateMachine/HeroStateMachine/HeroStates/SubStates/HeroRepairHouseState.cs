using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroRepairHouseState : HeroIdleState
{
    private HouseController targetHouseController;
    private float repairTimer = 0f;

    public HeroRepairHouseState(Hero hero, HeroStateMachine stateMachine, HeroData heroData, string animBoolName) : base(hero, stateMachine, heroData, animBoolName)
    {
    }

    public override void DoChecks() {
        base.DoChecks();
    }

    public override void Enter() {
        base.Enter();
        //damaged house was already selected as target
        targetHouseController = hero.targetGO.GetComponent<HouseController>();

        //get move command to damaged house
        hero.MovementController.GetMoveCommand(hero.targetGO.transform.position);
        turnSpeed = heroData.patrolTurnSpeed;
    }

    public override void Exit() {
        base.Exit();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();
        if (targetHouseController.health == targetHouseController.maxHealth || targetHouseController.destroyed) {
            //house repaired or was somehow destroyed, so the job's done
            stateMachine.ChangeState(hero.PatrolState);
            hero.targetGO = null; //no longer targeting house
        }

        if(Vector2.Distance(hero.transform.position, targetHouseController.transform.position) < 0.5f) {
            repairTimer += Time.deltaTime;
            if(repairTimer >= heroData.repairTime) {
                //apply repair
                //reset timer
                targetHouseController.RepairDamage();
                repairTimer = 0f;
            }
            
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();

        //look towards direction of movement
        lookAngle = Vector2.SignedAngle(Vector2.up, hero.MovementController.intendedVelocity);
    }
}
