using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroStunnedState : HeroCombatState
{
    public HeroStunnedState(Hero hero, HeroStateMachine stateMachine, HeroData heroData, string animBoolName) : base(hero, stateMachine, heroData, animBoolName) {
    }

    public override void DoChecks() {
        base.DoChecks();
    }

    public override void Enter() {
        base.Enter();
        hero.MovementController.enabled = false;
    }

    public override void Exit() {
        base.Exit();
        hero.MovementController.enabled = true;
    }

    public override void LogicUpdate() {
        base.LogicUpdate();
        if(Time.time - startTime >= heroData.stunTime) {
            if (hero.targetGO != null)
                stateMachine.ChangeState(hero.ChaseState);
            else
                stateMachine.ChangeState(hero.PatrolState);
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
        hero.RB.velocity = Vector2.Lerp(hero.RB.velocity, Vector2.zero, (Time.time - startTime) / heroData.stunTime);
    }
}
