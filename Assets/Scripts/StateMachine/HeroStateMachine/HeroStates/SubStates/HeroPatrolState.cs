using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPatrolState : HeroIdleState
{
    /// <summary>
    /// Index of current patrol point.
    /// </summary>
    private int patrolIndex = 0;
    private bool isAtPatrolPoint = false;
    private float lookTimer = 0f;
    private float lookSubUpdateTime = 0.2f; //prevents jittering
    private float lookSubUpdateTimer = 0f;
    private bool lookLeft = false; //alternates direction every time
    private float offsetAngle = 0f; //use to calculate angle between updates
    private bool lookingAtPlayer = false;
    public HeroPatrolState(Hero hero, HeroStateMachine stateMachine, HeroData heroData, string animBoolName) : base(hero, stateMachine, heroData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
        if (hero.PatrolPoints.Count > 0) {
            isAtPatrolPoint = CheckIfAtPoint(hero.PatrolPoints[patrolIndex]);
        }
    }

    public override void Enter()
    {
        base.Enter();
        //get move command
        if (hero.PatrolPoints.Count > 0) {
            hero.MovementController.GetMoveCommand(hero.PatrolPoints[patrolIndex]);
        }
        turnSpeed = heroData.patrolTurnSpeed;

        lookTimer = heroData.patrolLookTime; //look right away.
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(isAtPatrolPoint){
            stateMachine.ChangeState(hero.LookAroundState);
            SelectNextPatrolPoint();
        }

        if (DamagedHouseVisible()) {
            stateMachine.ChangeState(hero.RepairHouseState);
        }


        lookTimer += Time.deltaTime;
        lookSubUpdateTimer += Time.deltaTime;
        Vector2 dir = hero.MovementController.intendedVelocity;
        
        if (lookTimer >= heroData.patrolLookTime) {
            //switch sides
            lookLeft = !lookLeft;
            if (dir != Vector2.zero) {
                lookAngle = Vector2.SignedAngle(Vector2.up, dir);
                offsetAngle = lookAngle; //save "movement angle:
                //lookAngle = Vector2.SignedAngle(Vector2.up, dir);
                lookAngle += lookLeft ? 45f : -45f;

                if (!hero.IsLookAngleValid(lookAngle)) {
                    lookAngle = hero.ChooseBetterLookAngle(lookAngle);
                }

                //we have the desired angle, let's find what the offset is from the movement angle to use it between angle changes
                offsetAngle = lookAngle - offsetAngle;

            }
            lookTimer = 0f;
        }
        if(lookSubUpdateTimer >= lookSubUpdateTime) {
            //lookangle should be relative to movement direction
            lookAngle = Vector2.SignedAngle(Vector2.up, dir); //recalculate movement angle
            lookAngle += offsetAngle; //add offset

            lookSubUpdateTimer = 0f;
        }

        if (isPlayerClose) {
            //roll the dice to see if you should look there
            if(Random.Range(0, 100) > 90) { //1 in 10
                dir = GameObject.FindObjectOfType<PlayerController>().transform.position - hero.transform.position;
                lookAngle = Vector2.SignedAngle(Vector2.up, dir);

                //reset timers
                lookTimer = 0f;
                lookSubUpdateTimer = 0f;
            }
        }

        
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        //look towards direction of movement
        //lookAngle = Vector2.SignedAngle(Vector2.up, hero.MovementController.intendedVelocity);
    }

    public override void Exit()
    {
        base.Exit();
    }

    private bool CheckIfAtPoint(Vector2 point){
        return Vector2.Distance(hero.transform.position, point) < 0.5f;
    }

    private void SelectNextPatrolPoint(){
        patrolIndex++;
        if(patrolIndex >= hero.PatrolPoints.Count){
            patrolIndex = 0;
        }
    }
}
