using System.Collections;
using UnityEngine;


public class HeroSearchState : HeroCombatState
{
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
        hero.MovementController.GetMoveCommand(targetLastKnownPosition);
    }

    public override void Exit() {
        base.Exit();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();
        
        if(Time.time - startTime >= heroData.searchTime) {
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
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
}