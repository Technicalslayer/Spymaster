using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroConfuseState : HeroState
{
    public HeroConfuseState(Hero hero, HeroStateMachine stateMachine, HeroData heroData, string animBoolName) : base(hero, stateMachine, heroData, animBoolName) {
    }

    public override void DoChecks() {
        base.DoChecks();
    }

    public override void Enter() {
        base.Enter();
    }

    public override void Exit() {
        base.Exit();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();
        if(Time.time - startTime >= heroData.confuseTime) {
            stateMachine.ChangeState(hero.PatrolState);
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
        hero.RB.velocity = Vector2.Lerp(hero.RB.velocity, Vector2.zero, (Time.time - startTime) / heroData.stunTime);
    }
}
