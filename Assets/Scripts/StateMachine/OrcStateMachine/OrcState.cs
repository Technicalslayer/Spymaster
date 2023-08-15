using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base state, inherited by all other hero states.
/// </summary>
public class OrcState { 
    protected Orc orc;
    protected OrcStateMachine stateMachine;
    protected OrcData orcData;

    //protected float lookAngle;
    //protected float turnSpeed;

    /// <summary>
    /// Tracks when state was entered, so that you can see how long a state has been active.
    /// </summary>
    protected float startTime;
    private string animBoolName; //may not use

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="orc"></param>
    /// <param name="stateMachine"></param>
    /// <param name="orcData"></param>
    /// <param name="animBoolName"></param>
    public OrcState(Orc orc, OrcStateMachine stateMachine, OrcData orcData, string animBoolName) {
        this.orc = orc;
        this.stateMachine = stateMachine;
        this.orcData = orcData;
        this.animBoolName = animBoolName;
    }

    /// <summary>
    /// Calls several check functions at once. Called at Enter, and each Physics Update.
    /// <br>Should not change states/logic directly. Only update check variables.</br>
    /// </summary>
    public virtual void DoChecks() { }
    /// <summary>
    /// Called when entering state
    /// </summary>
    public virtual void Enter() {
        DoChecks();
        startTime = Time.time;
        //orc.Anim.SetBool(animBoolName, true);
        Debug.Log(animBoolName);
    }
    /// <summary>
    /// Called when exiting state
    /// </summary>
    public virtual void Exit() { }
    /// <summary>
    /// Called every Update cycle
    /// </summary>
    public virtual void LogicUpdate() { }
    /// <summary>
    /// Called every FixedUpdate cycle
    /// </summary>
    public virtual void PhysicsUpdate() {
        DoChecks();
    }
}
