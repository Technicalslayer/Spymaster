using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores reference to current state
/// </summary>
public class HeroStateMachine
{
    public HeroState CurrentState { get; private set; }

    public void Initialize(HeroState startingState){
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(HeroState newState){
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
