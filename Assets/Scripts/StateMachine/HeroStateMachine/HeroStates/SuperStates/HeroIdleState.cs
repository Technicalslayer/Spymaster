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
        

        bool playerVisible = false;
        if(hero.visibleEnemies.Length > 0) {
            //if can see player, increase detection meter.
            foreach(GameObject enemy in hero.visibleEnemies) {
                if (enemy.CompareTag("Player")) {
                    playerVisible = true;
                    hero.IncrementDetection(1f * Time.deltaTime);
                    //if detection meter full, then chase player.
                    if(hero.GetDetectionProgress() >= 1f) {
                        hero.targetGO = enemy;
                        stateMachine.ChangeState(hero.ChaseState);
                        hero.Anim.Play("PlayerDetected");
                        return;
                    }
                }
                else { //not a player, should chase it
                    hero.targetGO = enemy;
                    stateMachine.ChangeState(hero.ChaseState);
                }
            }
        }

        if (!playerVisible) {
            //decrement timer slowly
            hero.IncrementDetection(heroData.detectionDecreaseRate * Time.deltaTime);
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
