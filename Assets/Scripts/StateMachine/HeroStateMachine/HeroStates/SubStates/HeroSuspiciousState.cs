using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSuspiciousState : HeroIdleState
{
    protected GameObject playerGO;
    protected Vector2 playerPosition;
    public HeroSuspiciousState(Hero hero, HeroStateMachine stateMachine, HeroData heroData, string animBoolName) : base(hero, stateMachine, heroData, animBoolName) {
    }

    public override void DoChecks() {
        base.DoChecks();
    }

    public override void Enter() {
        base.Enter();
        //get player reference (lazy, but whatever)
        playerGO = Object.FindObjectOfType<PlayerController>().gameObject;
        turnSpeed = heroData.suspiciousTurnSpeed;
        playerPosition = playerGO.transform.position;
        hero.MovementController.GetMoveCommand(playerPosition); //go towards where you saw them
    }

    public override void Exit() {
        base.Exit();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        if (hero.TargetInViewRange(playerGO)) {
            hero.IncrementDetection(1f * Time.deltaTime);
            //if detection meter full, then chase player.
            if (hero.GetDetectionProgress() >= 1f) {
                hero.targetGO = playerGO;
                stateMachine.ChangeState(hero.ChaseState);
                hero.Anim.Play("PlayerDetected");
                hero.ResetDetectionProgress();
                return;
            }
        }
        else {
            //decrement timer slowly
            hero.IncrementDetection(heroData.detectionDecreaseRate * Time.deltaTime);
        }

        //look around relative to where the player was
        //lookAngle = get angle to player, pick random direction within some offset of that
        //after some time, look somewhere else or update angle to player?


        if(Time.time - startTime > heroData.suspiciousTime) {
            if(hero.GetDetectionProgress() > 0f) {
                //still searching, restart Timer
                startTime = Time.time;
                hero.MovementController.GetMoveCommand(playerGO.transform.position);
            }
            else {
                stateMachine.ChangeState(hero.PatrolState);
            }
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }

    private void SomethingToDo() {
        //if (lookTarget != null) {
        //    lookAngle = Vector2.SignedAngle(Vector2.up, lookTarget.transform.position - hero.transform.position);
        //}
    }
}
