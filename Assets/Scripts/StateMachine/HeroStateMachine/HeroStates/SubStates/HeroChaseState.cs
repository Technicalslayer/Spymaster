using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroChaseState : HeroCombatState
{
    private float movementUpdateTimer = 0f;
    private float chaseSearchTimer = 0f; //how long the target has been out of sight

    public HeroChaseState(Hero hero, HeroStateMachine stateMachine, HeroData heroData, string animBoolName) : base(hero, stateMachine, heroData, animBoolName)
    {
    }

    public override void DoChecks() {
        base.DoChecks();
    }

    public override void Enter() {
        base.Enter();
        turnSpeed = heroData.chaseTurnSpeed;

        //pick target to chase
        //if(hero.targetGO != null) {
        //    if (!ChooseClosestEnemyTarget(ref hero.targetGO)) {
        //        //couldn't find enemy
        //        Debug.LogError("Couldn't find an enemy when entering HeroChaseState.");
        //        stateMachine.ChangeState(hero.PatrolState);
        //        return; //do nothing else in this state
        //    }
        //}
        //should only enter chase state if I have a target
        if(hero.targetGO == null) {
            Debug.LogError("Didn't have an enemy when entering HeroChaseState.");
            //stateMachine.ChangeState(hero.PatrolState);
            //return;
        }

        //initial move command
        hero.MovementController.GetMoveCommand(hero.targetGO.transform.position);
        hero.targetLastKnownLocation = hero.targetGO.transform.position;

        //if chasing player, turn off players spy view
        if(hero.targetGO.CompareTag("Player")) {
            hero.targetGO.GetComponent<PlayerController>().DisableFoV();
        }
    }

    public override void Exit() {
        base.Exit();
        //always enable fov just incase
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().EnableFoV();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        movementUpdateTimer += Time.deltaTime;
        if(movementUpdateTimer >= heroData.movementUpdateTime) {
            movementUpdateTimer = 0f;
            hero.MovementController.GetMoveCommand(hero.targetGO.transform.position);
        }
        //check if target is visible
        if(hero.TargetInViewRange(hero.targetGO)) {
            //hero.targetLastKnownLocation = hero.targetGO.transform.position;
            chaseSearchTimer = 0f; //reset timer
        }
        else {
            //lost sight
            chaseSearchTimer += Time.deltaTime;
            if(chaseSearchTimer >= heroData.chaseSearchTime) {
                chaseSearchTimer = 0f;
                hero.targetLastKnownLocation = hero.targetGO.transform.position; //clairvoyant hero
                stateMachine.ChangeState(hero.SearchState);
            }
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();

        //look towards direction of movement
        lookAngle = Vector2.SignedAngle(Vector2.up, hero.MovementController.intendedVelocity);
    }

    

}
