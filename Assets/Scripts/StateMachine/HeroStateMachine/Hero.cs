using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    
    #region State Variables
    public HeroStateMachine StateMachine {get; private set; }
    public HeroPatrolState PatrolState { get; private set; }
    public HeroLookAroundState LookAroundState { get; private set; }
    public HeroRepairHouseState RepairHouseState { get; private set; }

    [SerializeField]
    private HeroData heroData;
    #endregion


    #region Components
    public MovementController2D MovementController { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public FieldOfView FoV { get; private set; }
    #endregion

    #region Variables
    public List<Vector2> PatrolPoints;
    #endregion


    #region Unity Callbacks
    private void Awake(){
        StateMachine = new HeroStateMachine();

        PatrolState = new HeroPatrolState(this, StateMachine, heroData, "patrol");
        LookAroundState = new HeroLookAroundState(this, StateMachine, heroData, "look");
        RepairHouseState = new HeroRepairHouseState(this, StateMachine, heroData, "repair");

        //get components
        MovementController = GetComponent<MovementController2D>();
        RB = GetComponent<Rigidbody2D>();
        FoV = GetComponentInChildren<FieldOfView>();
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
    #endregion

    #region Functions
    public void TurnTowardsAngle(float desiredAngle, float turnSpeed){
        float currentAngle = RB.rotation;
        float rotateStepSize = turnSpeed * Time.fixedDeltaTime;
        //rb.MoveRotation(Mathf.LerpAngle(currentAngle, desiredAngle, 0.05f));
        if(Mathf.Abs(Mathf.DeltaAngle(currentAngle, desiredAngle)) > rotateStepSize){
            //move towards desired angle at set speed
            RB.MoveRotation(Mathf.MoveTowardsAngle(currentAngle, desiredAngle, rotateStepSize));
        }
        else{
            //too close for step, snap to angle
            RB.MoveRotation(desiredAngle);
        }
    }
    #endregion

}
