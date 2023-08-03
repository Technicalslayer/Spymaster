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
    public HeroPatrolState(Hero hero, HeroStateMachine stateMachine, HeroData heroData, string animBoolName) : base(hero, stateMachine, heroData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isAtPatrolPoint = CheckIfAtPoint(hero.PatrolPoints[patrolIndex]);
    }

    public override void Enter()
    {
        base.Enter();
        //get move command
        hero.MovementController.GetMoveCommand(hero.PatrolPoints[patrolIndex]);
        turnSpeed = heroData.patrolTurnSpeed;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(isAtPatrolPoint){
            stateMachine.ChangeState(hero.LookAroundState);
            SelectNextPatrolPoint();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        //look towards direction of movement
        lookAngle = Vector2.SignedAngle(Vector2.up, hero.MovementController.intendedVelocity);
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
