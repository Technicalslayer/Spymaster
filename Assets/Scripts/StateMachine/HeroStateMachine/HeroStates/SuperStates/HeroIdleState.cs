using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class HeroIdleState : HeroState
{
    //protected GameObject lookTarget; //influences the direction the hero looks in
    protected bool isPlayerClose;
    public HeroIdleState(Hero hero, HeroStateMachine stateMachine, HeroData heroData, string animBoolName) : base(hero, stateMachine, heroData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
        hero.GetAllTargetsInViewRange();

        //check if player is too close and not behind something
        GameObject player = GameObject.FindObjectOfType<PlayerController>().gameObject;
        Vector2 dir = player.transform.position - hero.transform.position;
        //raycast
        RaycastHit2D hit = Physics2D.Raycast(hero.transform.position, dir, Mathf.Infinity, heroData.targetLayers | heroData.obstacleLayer);
        if (hit) {
            if (ReferenceEquals(hit.collider.gameObject, player)) {
                //this implies no obstacles were in the way
                if (hit.distance < heroData.playerProximityRange) {
                    isPlayerClose = true;
                }
                else {
                    isPlayerClose = false;
                }
            }
        }
        
        
    }

    public override void Enter()
    {
        base.Enter();
        hero.MovementController.speed = heroData.patrolSpeed;

        hero.Anim.Play("ResetDetection");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        

        
        if(hero.visibleEnemies.Length > 0) {
            //if can see player, increase detection meter.
            foreach(GameObject enemy in hero.visibleEnemies) {
                if (enemy.CompareTag("Player")) {
                    if(stateMachine.CurrentState != hero.SuspiciousState)
                        stateMachine.ChangeState(hero.SuspiciousState);
                }
                else { //not a player, should chase it
                    hero.targetGO = enemy;
                    stateMachine.ChangeState(hero.ChaseState);
                }
            }
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
