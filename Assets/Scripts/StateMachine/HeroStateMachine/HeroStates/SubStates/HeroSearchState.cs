using System.Collections;
using UnityEngine;


public class HeroSearchState : HeroCombatState
{
    private float lookTime; //how long hero has been looking in a single direction
    private float maxLookTime = 0.5f; //how long hero should look before changing directions

    public HeroSearchState(Hero hero, HeroStateMachine stateMachine, HeroData heroData, string animBoolName) : base(hero, stateMachine, heroData, animBoolName) {
    }

    public override void DoChecks() {
        base.DoChecks();
        hero.GetAllTargetsInViewRange();
    }

    public override void Enter() {
        base.Enter();
        hero.MovementController.speed = heroData.chaseSpeed;
        //go to last known location
        hero.MovementController.GetMoveCommand(hero.targetLastKnownLocation);
        lookTime = maxLookTime; //start looking immediately
    }

    public override void Exit() {
        base.Exit();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();
        

        if (Time.time - startTime >= heroData.searchTime) {
            //give up
            stateMachine.ChangeState(hero.PatrolState);
            hero.targetGO = null;
        }

        //if enemy visible, chase it
        if (hero.visibleEnemies.Length > 0) {
            //chase that target
            hero.targetGO = hero.visibleEnemies[0];
            stateMachine.ChangeState(hero.ChaseState);
        }

        //if(Vector3.Distance(hero.transform.position, targetLastKnownPosition) < 0.5f) {
        lookTime += Time.deltaTime;
        if (lookTime >= maxLookTime) {
            PickRandomAngleAndTime();
        }
        //}
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }

    private void PickRandomAngleAndTime() {
        maxLookTime = Random.Range(heroData.minLookTime, heroData.maxLookTime);
        lookTime = 0f;
        lookAngle = hero.ChooseRandomLookAngle();
        turnSpeed = Random.Range(heroData.minTurnSpeed, heroData.maxTurnSpeed);
    }
}