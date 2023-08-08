using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Hero : MonoBehaviour
{
    
    #region State Variables
    public HeroStateMachine StateMachine {get; private set; }
    public HeroPatrolState PatrolState { get; private set; }
    public HeroLookAroundState LookAroundState { get; private set; }
    public HeroRepairHouseState RepairHouseState { get; private set; }
    public HeroChaseState ChaseState { get; private set; }

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
    [HideInInspector]
    public GameObject targetGO;
    [HideInInspector]
    public GameObject[] visibleTargets;
    #endregion


    #region Unity Callbacks
    private void Awake(){
        StateMachine = new HeroStateMachine();

        PatrolState = new HeroPatrolState(this, StateMachine, heroData, "patrol");
        LookAroundState = new HeroLookAroundState(this, StateMachine, heroData, "look");
        RepairHouseState = new HeroRepairHouseState(this, StateMachine, heroData, "repair");
        ChaseState = new HeroChaseState(this, StateMachine, heroData, "chase");

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

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.collider.tag == "Orc"){
            StateMachine.ChangeState(ChaseState);
            targetGO = other.gameObject;
        }
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

    public bool TargetInViewRange(Vector3 targetPos, string targetTag) {
        bool inAngle;
        bool inRange;
        bool correctTag;

        //calc angle
        Vector2 dir = targetPos - transform.position;
        float angleToTarget = Vector2.Angle(transform.up, dir);

        //raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, FoV.viewDistance, heroData.targetLayers | heroData.obstacleLayer);
        if (hit && hit.collider.tag == targetTag) {
            correctTag = true; //this implies no obstacles were in the way
        }
        else {
            correctTag = false;
        }
        Debug.DrawRay(transform.position, dir, Color.red);

        inAngle = angleToTarget < FoV.viewAngle / 2;
        inRange = Vector2.Distance(transform.position, targetPos) < FoV.viewDistance;
        //Debug.Log(correctTag + " " +  inAngle + " " + inRange);
        return correctTag && inAngle && inRange;
    }

    public void GetAllTargetsInViewRange() {
        List<GameObject> tempTargets = new List<GameObject>();
        Collider2D[] colliders;

        //find all targets in view radius
        colliders = Physics2D.OverlapCircleAll(transform.position, FoV.viewDistance, heroData.targetLayers);

        //check which targets are in line of sight and in angle
        for(int i = 0; i < colliders.Length; i++) {
            Vector2 dir = colliders[i].transform.position - transform.position;
            float angleToTarget = Vector2.Angle(transform.up, dir);
            if(angleToTarget < FoV.viewAngle / 2) {
                //target within angles, so check line of sight
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, dir, FoV.viewDistance, heroData.targetLayers | heroData.obstacleLayer);
                foreach(RaycastHit2D hit in hits) {
                    int tempLayer = hit.collider.gameObject.layer;
                    if(ReferenceEquals(hit.collider.gameObject, colliders[i].gameObject)) {
                        //hit target and haven't hit an obstacle yet, so valid, save and move to next potential target
                        tempTargets.Add(colliders[i].gameObject);
                        break;
                    }
                    else if(tempLayer == LayerMask.NameToLayer("Obstacles") || tempLayer == LayerMask.NameToLayer("Houses")) {
                        //invalid, can't see target, move to next potential target
                        break;
                    }
                    //didn't hit target or obstacle, moving on to next object in line
                }
            }
        }
        visibleTargets = tempTargets.ToArray();
    }

    #endregion

}
