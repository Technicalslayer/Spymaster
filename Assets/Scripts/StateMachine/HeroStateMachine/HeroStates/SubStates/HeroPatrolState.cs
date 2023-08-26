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
    private bool lookLeft = false; //alternates direction every time
    private float lookOffset = 0f;
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
        if (lookTimer >= heroData.patrolLookTime) {
            //switch sides
            lookLeft = !lookLeft;
            Vector2 dir = hero.MovementController.intendedVelocity;
            if (dir != Vector2.zero) {
                //lookAngle = Vector2.SignedAngle(Vector2.up, dir);
                lookAngle = Vector2.SignedAngle(Vector2.up, dir);
                lookAngle += lookLeft ? 45f : -45f;
                
                if (!hero.IsLookAngleValid(lookAngle)) {
                    lookAngle = hero.ChooseBetterLookAngle(lookAngle);
                }

            }
            lookTimer = 0f;
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
