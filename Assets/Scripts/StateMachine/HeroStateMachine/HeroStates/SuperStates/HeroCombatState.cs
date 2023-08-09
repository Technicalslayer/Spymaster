using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCombatState : HeroState
{
    protected Vector3 targetLastKnownPosition;

    public HeroCombatState(Hero hero, HeroStateMachine stateMachine, HeroData heroData, string animBoolName) : base(hero, stateMachine, heroData, animBoolName) {
    }

    public override void DoChecks() {
        base.DoChecks();
    }

    public override void Enter() {
        base.Enter();
        hero.MovementController.speed = heroData.chaseSpeed;
    }

    public override void Exit() {
        base.Exit();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
}
