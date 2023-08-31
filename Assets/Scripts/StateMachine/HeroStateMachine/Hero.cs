using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Hero : MonoBehaviour
{
    
    #region State Variables
    public HeroStateMachine StateMachine {get; private set; }
    public HeroPatrolState PatrolState { get; private set; }
    public HeroLookAroundState LookAroundState { get; private set; }
    public HeroRepairHouseState RepairHouseState { get; private set; }
    public HeroChaseState ChaseState { get; private set; }
    public HeroSearchState SearchState { get; private set; }
    public HeroStunnedState StunnedState { get; private set; }
    public HeroConfuseState ConfuseState { get; private set; }
    public HeroSuspiciousState SuspiciousState { get; private set; }

    [SerializeField]
    private HeroData heroData;
    #endregion


    #region Components
    public MovementController2D MovementController { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public FieldOfView FoV { get; private set; }
    public Animator Anim { get; private set; }
    public Image detectionImage;
    public AudioSource repairSound;
    #endregion

    #region Variables
    public List<Vector2> PatrolPoints;
    [HideInInspector]
    public GameObject targetGO;
    /// <summary>
    /// Should always be sorted by distance.
    /// </summary>
    [HideInInspector]
    public GameObject[] visibleHouses;
    /// <summary>
    /// Should always be sorted by distance
    /// </summary>
    [HideInInspector]
    public GameObject[] visibleEnemies;
    [HideInInspector]
    public Vector3 targetLastKnownLocation;
    [HideInInspector]
    public float detectionProgress; //fully detected once equal to detectionTime
    #endregion

    //private IEnumerator InitDelay() {
    //    yield return null;
    //    StateMachine.Initialize(PatrolState);
    //}

    #region Unity Callbacks
    private void Awake(){
        StateMachine = new HeroStateMachine();

        PatrolState = new HeroPatrolState(this, StateMachine, heroData, "patrol");
        LookAroundState = new HeroLookAroundState(this, StateMachine, heroData, "look");
        RepairHouseState = new HeroRepairHouseState(this, StateMachine, heroData, "repair");
        ChaseState = new HeroChaseState(this, StateMachine, heroData, "chase");
        SearchState = new HeroSearchState(this, StateMachine, heroData, "search");
        StunnedState = new HeroStunnedState(this, StateMachine, heroData, "stunned");
        ConfuseState = new HeroConfuseState(this, StateMachine, heroData, "confused");
        SuspiciousState = new HeroSuspiciousState(this, StateMachine, heroData, "suspicious");

        //get components
        MovementController = GetComponent<MovementController2D>();
        RB = GetComponent<Rigidbody2D>();
        FoV = GetComponentInChildren<FieldOfView>();
        Anim = GetComponent<Animator>();
        repairSound = GetComponent<AudioSource>();
    }

    private void Start() {
        StateMachine.Initialize(PatrolState);
        //wait a frame for some bullshit
        //StartCoroutine(InitDelay());
    }

    private void Update() {
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate() {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(StateMachine.CurrentState == ConfuseState) {
            return;
        }

        if(other.collider.tag == "Orc"){
            //apply impulse
            if (other.contactCount > 0)
                RB.AddForce(other.contacts[0].normal * 10f, ForceMode2D.Impulse);
            targetGO = other.gameObject;
            StateMachine.ChangeState(StunnedState);
        }
        //else if(other.collider.tag == "House") {
        //    if(StateMachine.CurrentState == RepairHouseState) {
        //        //apply impulse
        //        //if (other.contactCount > 0)
        //        //    RB.AddForce(other.contacts[0].normal * 10f, ForceMode2D.Impulse);
        //        //send signal to repair house?
        //    }
        //}
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (collision.collider.tag == "Orc" && StateMachine.CurrentState != StunnedState) {
            //apply impulse
            if (collision.contactCount > 0)
                RB.AddForce(collision.contacts[0].normal * 10f, ForceMode2D.Impulse);
            targetGO = collision.gameObject;
            StateMachine.ChangeState(StunnedState);
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

    /// <summary>
    /// Checks if specific target is visible
    /// </summary>
    /// <param name="targetObject"></param>
    /// <returns></returns>
    public bool TargetInViewRange(GameObject targetObject) {
        bool inAngle = false;
        bool inRange = false;
        bool lineOfSight = false;

        Vector3 targetPos = targetObject.transform.position;
        //calc angle
        Vector2 dir = targetPos - transform.position;
        float angleToTarget = Vector2.Angle(transform.up, dir);

        //raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, FoV.viewDistance, heroData.targetLayers | heroData.obstacleLayer);
        if (hit) {
            if (ReferenceEquals(hit.collider.gameObject, targetObject)) {
                lineOfSight = true; //this implies no obstacles were in the way
            }
            else {
                lineOfSight = false;
            }
        }
        Debug.DrawRay(transform.position, dir, Color.red);

        inAngle = angleToTarget < FoV.viewAngle / 2;
        inRange = Vector2.Distance(transform.position, targetPos) < FoV.viewDistance;
        //Debug.Log(correctTag + " " +  inAngle + " " + inRange);
        return lineOfSight && inAngle && inRange;
    }

    /// <summary>
    /// Finds and stores all visible targets
    /// </summary>
    public void GetAllTargetsInViewRange() {
        List<GameObject> tempHouses = new List<GameObject>();
        List<GameObject> tempEnemies = new List<GameObject>();
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
                        //check if target is an enemy
                        if (colliders[i].tag == "Orc" || colliders[i].tag == "Player") {
                            tempEnemies.Add(colliders[i].gameObject);
                        }
                        else if (colliders[i].tag == "House") {
                            tempHouses.Add(colliders[i].gameObject);
                        }
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
        visibleEnemies = tempEnemies.ToArray();
        visibleHouses = tempHouses.ToArray();
    }

    public void ApplyConfusion() {
        if(StateMachine.CurrentState != ConfuseState) {
            StateMachine.ChangeState(ConfuseState);
        }
    }

    public void IncrementDetection(float amount) {
        detectionProgress += amount;
        detectionProgress = Mathf.Clamp(detectionProgress, 0f, heroData.detectionTime);
        float t = GetDetectionProgress();
        detectionImage.fillAmount = t;
        //Debug.Log(t);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>A float between 0 and 1</returns>
    public float GetDetectionProgress() {
        return detectionProgress / heroData.detectionTime;
    }

    public void ResetDetectionProgress() {
        detectionProgress = 0f;
        detectionImage.fillAmount = 0f;
    }
    
    //returns true if angle is valid
    public bool IsLookAngleValid(float angle) {
        //raycast
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)), FoV.viewDistance, heroData.obstacleLayer);
        float angleT = angle;
        Quaternion rotation = Quaternion.Euler(0, 0, angleT);
        Vector2 result = rotation * Vector2.up;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, result, FoV.viewDistance, heroData.obstacleLayer);
        Debug.DrawRay(transform.position, result * hit.distance, Color.blue, 2f);
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(angleT * Mathf.Deg2Rad), Mathf.Sin(angleT * Mathf.Deg2Rad)), FoV.viewDistance, heroData.obstacleLayer);
        //Debug.DrawLine(transform.position, transform.position + (Vector3)new Vector2(Mathf.Cos(angleT * Mathf.Deg2Rad), Mathf.Sin(angleT * Mathf.Deg2Rad)) * hit.distance, Color.red, 1f);
        //Debug.Log("X: " + Mathf.Cos(angleT * Mathf.Deg2Rad) + ", Y: " + Mathf.Sin(angleT * Mathf.Deg2Rad) + ", Angle: " + angleT);
        //Debug.Log("X: " + result.x + ", Y: " + result.y + ", Angle: " + angleT + ", Original Angle: " + angle);
        //check distance to wall/obstacle
        if (!hit || hit.distance > heroData.minWallLookDistance) {
            //valid
            return true;
        }
        

        return false;
    }

    public float ChooseBetterLookAngle(float angle) {
        //called if our look angle was invalid, but we still want to look in that general direction
        for (int i = 0; i < 3; i++) { //3 checks on each side
            for (int j = 1; j >= -1; j -= 2) { //alternate negative and positive.
                float angleOffset = i * 15f * j; //15 * 3 * 2 = 90f. We start small to stay close to desired angle.
                float newAngle = angle + angleOffset;
                Quaternion rotation = Quaternion.Euler(0, 0, newAngle);
                Vector2 result = rotation * Vector2.up;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, result, FoV.viewDistance, heroData.obstacleLayer);
                //Debug.DrawRay(transform.position, result * hit.distance, Color.green, 2f);
                //float angleT = newAngle + 90f; //offset because of character rotation shenanigans
                //RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(angleT * Mathf.Deg2Rad), Mathf.Sin(angleT * Mathf.Deg2Rad)), FoV.viewDistance, heroData.obstacleLayer);
                //Debug.DrawLine(transform.position, transform.position + (Vector3)new Vector2(Mathf.Cos(angleT * Mathf.Deg2Rad), Mathf.Sin(angleT * Mathf.Deg2Rad)) * hit.distance, Color.green, 1f);
                //check distance to wall/obstacle
                if (hit) {
                    if (hit.distance > heroData.minWallLookDistance) {
                        //angle is valid
                        //Debug.Log("Found Better Angle: " + newAngle + ", Old Angle: " + angle);
                        return newAngle;
                    }
                }
                else {
                    //angle is valid cuz nothing in way
                    //Debug.Log("Found Better Angle: " + newAngle + ", Old Angle: " + angle);
                    return newAngle;
                }
            }
        }
        //couldn't find better option, return original
        //Debug.Log("Couldn't find a better angle");
        return angle;
    }

    public float ChooseRandomLookAngle() {
        //raycast in multiple directions
        float angle = 0;
        List<float> validAngles = new List<float>();
        for(int i = 0; i < 8; i++) {
            angle = i * 45f;
            angle += 90f; //offset because character is aligned with vertical instead of horizontal
            Quaternion rotation = Quaternion.Euler(0,0,angle);
            Vector2 result = rotation * Vector2.up;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, result, FoV.viewDistance, heroData.obstacleLayer);
            Debug.DrawRay(transform.position, result * hit.distance, Color.green, 2f);
            //float angleT = angle + 90f; //offset because of character rotation shenanigans
            //RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(angleT * Mathf.Deg2Rad), Mathf.Sin(angleT * Mathf.Deg2Rad)), FoV.viewDistance, heroData.obstacleLayer);
            //Debug.DrawLine(transform.position, transform.position + (Vector3)new Vector2(Mathf.Cos(angleT * Mathf.Deg2Rad), Mathf.Sin(angleT * Mathf.Deg2Rad)) * hit.distance, Color.yellow, 1f);
            //Debug.DrawRay(transform.position, new Vector2(Mathf.Cos(angleT * Mathf.Deg2Rad), Mathf.Sin(angleT * Mathf.Deg2Rad)) * hit.distance, Color.yellow, 1f);
            //RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)), FoV.viewDistance, heroData.obstacleLayer);
            //check distance to wall/obstacle
            if (!hit || hit.distance > heroData.minWallLookDistance) {
                //angle is valid
                validAngles.Add(angle);
            }
        }
        
        //return validAngles.Count > 0 ? validAngles[Random.Range(0, validAngles.Count)] * 45f : 0;
        if (validAngles.Count > 0) {
            return validAngles[Random.Range(0, validAngles.Count)];
        }
        return 0;
    }

    #endregion

}
