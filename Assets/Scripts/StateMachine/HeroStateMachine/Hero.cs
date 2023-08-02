using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public HeroStateMachine StateMachine {get; private set; }

    private void Awake(){
        StateMachine = new HeroStateMachine();
    }

    private void Start() {
        //TODO: Init statemachine
    }

    private void Update() {
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate() {
        StateMachine.CurrentState.PhysicsUpdate();
    }
}
