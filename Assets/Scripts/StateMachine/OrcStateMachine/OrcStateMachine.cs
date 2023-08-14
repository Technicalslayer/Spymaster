using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcStateMachine
{
    public OrcState CurrentState { get; private set; }

    public void Initialize(OrcState startingState) {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(OrcState newState) {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
