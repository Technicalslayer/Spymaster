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
        if(hero.visibleEnemies.Length > 0) {
            hero.targetGO = hero.visibleEnemies[0];
            stateMachine.ChangeState(hero.ChaseState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    protected bool DamagedHouseVisible() {
        foreach (var target in hero.visibleHouses) {
            HouseController tH;
            tH = target.GetComponent<HouseController>();
            if (!tH.destroyed && tH.health < tH.maxHealth) {
                hero.targetGO = target;
                return true; //found damaged house
            }
        }
        return false; //no houses
    }
}
