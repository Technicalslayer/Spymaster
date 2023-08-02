using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public HeroStateMachine StateMachine {get; private set; }

    //states
    public HeroPatrolState PatrolState { get; private set; }
    public HeroLookAroundState LookAroundState { get; private set; }
    public HeroRepairHouseState RepairHouseState { get; private set; }

    [SerializeField]
    private HeroData heroData;


    private void Awake(){
        StateMachine = new HeroStateMachine();

        PatrolState = new HeroPatrolState(this, StateMachine, heroData, "patrol");
        LookAroundState = new HeroLookAroundState(this, StateMachine, heroData, "look");
        RepairHouseState = new HeroRepairHouseState(this, StateMachine, heroData, "repair");
    }

    private void Start() {
        StateMachine.Initialize(PatrolState);
    }

    private void Update() {
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate() {
        StateMachine.CurrentState.PhysicsUpdate();
    }
}
