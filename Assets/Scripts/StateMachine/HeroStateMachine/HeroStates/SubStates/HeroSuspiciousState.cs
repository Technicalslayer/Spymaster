using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSuspiciousState : HeroIdleState
{
    protected GameObject playerGO;
    protected Vector2 playerPosition;

    private float lookTimer = 0f;
    private float lookTime = 0f;

    public HeroSuspiciousState(Hero hero, HeroStateMachine stateMachine, HeroData heroData, string animBoolName) : base(hero, stateMachine, heroData, animBoolName) {
    }

    public override void DoChecks() {
        base.DoChecks();
    }

    public override void Enter() {
        base.Enter();
        //get player reference (lazy, but whatever)
        playerGO = Object.FindObjectOfType<PlayerController>().gameObject;

        hero.MovementController.speed = heroData.suspiciousMoveSpeed; //override move speed
        turnSpeed = heroData.suspiciousTurnSpeed;
        playerPosition = playerGO.transform.position;
        lookTime = heroData.suspiciousLookTime;
        hero.MovementController.GetMoveCommand(playerPosition); //go towards where you saw them
        lookTimer = lookTime; //start looking around immediately
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

        lookTimer += Time.deltaTime;
        if(lookTimer >= heroData.suspiciousLookTime) {
            //look around relative to where the player was
            Vector2 dir = playerGO.transform.position - hero.transform.position;
            lookAngle = Vector2.SignedAngle(Vector2.up, dir);
            //check that angle isn't hitting wall
            if (!hero.IsLookAngleValid(lookAngle)) {
                //look in direction of movement
                lookAngle = Vector2.SignedAngle(Vector2.up, hero.MovementController.intendedVelocity);
            }
            //if hitting wall, look in direction of movement
            //lookAngle += Random.Range(-15f, 15f);
            //lookTime = Random.Range(0.2f, heroData.suspiciousLookTime);
            lookTimer = 0f;
        }


        if(Time.time - startTime >= heroData.suspiciousTime) {
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
