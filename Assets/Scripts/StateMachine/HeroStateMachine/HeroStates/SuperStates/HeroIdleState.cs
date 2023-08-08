using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroIdleState : HeroState
{
    public HeroIdleState(Hero hero, HeroStateMachine stateMachine, HeroData heroData, string animBoolName) : base(hero, stateMachine, heroData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
        hero.GetAllTargetsInViewRange();
    }

    public override void Enter()
    {
        base.Enter();
        hero.MovementController.speed = heroData.patrolSpeed;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        //TODO: UHGHGH
        if(hero.visibleTargets.Length > 0) {
            if (SelectEnemyTarget()) {
                stateMachine.ChangeState(hero.ChaseState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }


    protected bool SelectEnemyTarget() {
        foreach (var target in hero.visibleTargets) {
            if (target.tag == "Orc" || target.tag == "Player") {
                hero.targetGO = target;
                return true; //found closest enemy target already
            }
        }
        return false; //no enemies found
    }
}
