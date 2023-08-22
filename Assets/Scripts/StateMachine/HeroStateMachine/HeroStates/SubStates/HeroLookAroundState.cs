using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroLookAroundState : HeroIdleState
{
    private float lookTime; //how long hero has been looking in a single direction
    private float maxLookTime; //how long hero should look before changing directions
    

    public HeroLookAroundState(Hero hero, HeroStateMachine stateMachine, HeroData heroData, string animBoolName) : base(hero, stateMachine, heroData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        PickRandomAngleAndTime();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        //check how long I've been looking in a direction, then pick a new one
        lookTime += Time.deltaTime;
        if(lookTime >= maxLookTime){
            PickRandomAngleAndTime();
        }

        //if I've been in this state for a while, then exit and continue patrolling
        if(Time.time - startTime > 10f){
            stateMachine.ChangeState(hero.PatrolState);
        }

        if (DamagedHouseVisible()) {
            stateMachine.ChangeState(hero.RepairHouseState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private void PickRandomAngleAndTime(){
        maxLookTime = Random.Range(heroData.minLookTime, heroData.maxLookTime);
        lookTime = 0f;
        lookAngle = Vector2.SignedAngle(Vector2.up, Random.insideUnitCircle);
        turnSpeed = Random.Range(heroData.minTurnSpeed, heroData.maxTurnSpeed);
    }
}
