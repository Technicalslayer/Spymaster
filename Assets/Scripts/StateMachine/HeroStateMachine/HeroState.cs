using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base state, inherited by all other hero states.
/// </summary>
public class HeroState {
    protected Hero hero;
    protected HeroStateMachine stateMachine;
    protected HeroData heroData;

    protected float lookAngle;
    protected float turnSpeed;

    /// <summary>
    /// Tracks when state was entered, so that you can see how long a state has been active.
    /// </summary>
    protected float startTime; 
    private string animBoolName; //may not use

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="hero"></param>
    /// <param name="stateMachine"></param>
    /// <param name="heroData"></param>
    /// <param name="animBoolName"></param>
    public HeroState(Hero hero, HeroStateMachine stateMachine, HeroData heroData, string animBoolName){
        this.hero = hero;
        this.stateMachine = stateMachine;
        this.heroData = heroData;
        this.animBoolName = animBoolName;
    }

    /// <summary>
    /// Calls several check functions at once. Called at Enter, and each Physics Update.
    /// <br>Should not change states/logic directly. Only update check variables.</br>
    /// </summary>
    public virtual void DoChecks(){}
    /// <summary>
    /// Called when entering state
    /// </summary>
    public virtual void Enter(){
        DoChecks();
        startTime = Time.time;
        //hero.Anim.SetBool(animBoolName, true);
        Debug.Log(animBoolName);
    }
    /// <summary>
    /// Called when exiting state
    /// </summary>
    public virtual void Exit(){}
    /// <summary>
    /// Called every Update cycle
    /// </summary>
    public virtual void LogicUpdate(){}
    /// <summary>
    /// Called every FixedUpdate cycle
    /// </summary>
    public virtual void PhysicsUpdate(){
        DoChecks();
        hero.TurnTowardsAngle(lookAngle, turnSpeed);
    }
}
