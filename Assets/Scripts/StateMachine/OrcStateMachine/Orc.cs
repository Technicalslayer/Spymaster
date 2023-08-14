using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Orc : MonoBehaviour
{
    #region State Variables
    public OrcStateMachine StateMachine { get; private set; }
    public OrcAttackHouseState AttackHouseState { get; private set; }
    public OrcSeekHouseState SeekHouseState { get; private set; }
    public OrcStunnedState StunnedState { get; private set; }
    public OrcWaypointState WaypointState { get; private set; }
    public OrcIdleState IdleState { get; private set; }
    public OrcChaseState ChaseState { get; private set; }

    private OrcData orcData;
    #endregion

    #region Components
    public MovementController2D MovementController { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public TMP_Text hpText;

    #endregion

    #region Variables
    public GameObject targetGO;
    #endregion

    #region Unity Callbacks
    private void Awake() {
        StateMachine = new OrcStateMachine();

        //get components
        MovementController = GetComponent<MovementController2D>();
        RB = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        StateMachine.Initialize(SeekHouseState);
    }

    private void Update() {
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate() {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        
    }
    #endregion
}
