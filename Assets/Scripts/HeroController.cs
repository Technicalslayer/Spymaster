using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    public float rotateSpeed = 180f; //degrees per second?
    public float detectionRange = 100f; //how far to search for the player or orcs
    public float clashRange = 1f; //how close to begin charge
    public LayerMask raycastTargetLayer;
    public LayerMask raycastLayer;
    public List<Vector2> patrolPoints = new List<Vector2>();
    public Transform playerT;
    //public SpriteRenderer chaseIndicator; //sprite shows if hero is chasing player, has lost sight, or is wandering
    public Sprite chasingSprite;
    public Sprite lostSiteSprite;
    public Sprite wanderingSprite;

    private float searchTimer = 0f;
    private float searchTimerMax = 0.5f; //time in seconds between searches for player/orcs
    private Transform target; //target to make way towards
    private float stunTimer = 0f;
    private float stunTimerMax = 0.7f;
    private bool stunned = false;
    private bool chasing = false; //indicates hero is chasing player
    private bool playerInSight = false;
    private float wanderTimer = 0f;
    private float wanderTimeMax = 5f; //how long between selecting a new random point to travel to
    private int patrolPointIndex = 0; //current patrol target index?
    private float playerHideTimer = 0f; //how long has the player been out of sight?
    private float playerHideTimeMax = 5f; //how long for the player to be out of sight before giving up


    private MovementController2D movementController;
    private Rigidbody2D rb;
    private Collider2D c;
    private FieldOfView fieldOfView;

    // Start is called before the first frame update
    void Start()
    {
        //get components
        movementController = GetComponent<MovementController2D>();
        rb = GetComponent<Rigidbody2D>();
        c = GetComponent<Collider2D>();
        fieldOfView = GetComponentInChildren<FieldOfView>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (stunned) {
            stunTimer += Time.deltaTime;
            if (stunTimer > stunTimerMax) {
                stunned = false;
                stunTimer = 0f;
                //enable movement controller
                movementController.enabled = true;
            }
        }

        if (chasing) {
            //don't do anything other than chase player
            if (!playerInSight) {
                playerHideTimer += Time.deltaTime;
                if (playerHideTimer > playerHideTimeMax) {
                    //lost player, resume normal behavior
                    chasing = false;
                    //chaseIndicator.sprite = wanderingSprite;
                }
            }
        }
        else {
            //check if target still exists
            wanderTimer += Time.deltaTime;
            if (target == null && wanderTimer >= wanderTimeMax) {
                wanderTimer = 0f;
                //find new target or start wandering


                //start wandering
                if (patrolPoints.Count > 0) {
                    patrolPointIndex++;
                    if (patrolPointIndex >= patrolPoints.Count) {
                        patrolPointIndex = 0; //reset patrol
                    }
                    movementController.GetMoveCommand(patrolPoints[patrolPointIndex]);
                }
            }
        }
    }

    private void FixedUpdate() {
        if (stunned) {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, stunTimer / stunTimerMax); //slow down slide //I don't think this does anything cuz of the MovePosition function
        }
        else {
            //SearchForTarget();
            //rotate towards movement
            if (movementController.intendedVelocity != null) {
                float desiredAngle = Vector2.SignedAngle(Vector2.up, movementController.intendedVelocity);
                float currentAngle = rb.rotation;
                float rotateStepSize = rotateSpeed * Time.fixedDeltaTime;
                //rb.MoveRotation(Mathf.LerpAngle(currentAngle, desiredAngle, 0.05f));
                if(Mathf.Abs(Mathf.DeltaAngle(currentAngle, desiredAngle)) > rotateStepSize){
                    //move towards desired angle at set speed
                    rb.MoveRotation(Mathf.MoveTowardsAngle(currentAngle, desiredAngle, rotateStepSize));
                }
                else{
                    //too close for step, snap to angle
                    rb.MoveRotation(desiredAngle);
                }
            }
        }

        if (target != null) {
            //moving towards target
            searchTimer += Time.fixedDeltaTime;
            if (searchTimer > searchTimerMax) {
                searchTimer = 0f;
                movementController.GetMoveCommand(target.position);
            }
        }
        else {
            //move around randomly

        }
        // if (playerT) {
        //     Vector2 dir = playerT.position - transform.position;
        //     RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, raycastLayer);
        //     Debug.DrawRay(transform.position, dir, Color.red);

        //     if (hit && hit.collider.tag == "Player") {
        //         lineOfSight = true;
        //     }
        //     else {
        //         lineOfSight = false;
        //     }
        // }

        //check for targets
        RaycastHit2D[] targetsInViewDistance = Physics2D.CircleCastAll(transform.position, fieldOfView.viewDistance, Vector2.zero, 0f, raycastLayer);
        //check what targets are valid
        for(int i = 0; i < targetsInViewDistance.Length; i++){
            //check for player first
            if(TargetInViewRange(targetsInViewDistance[i].point, "Player")){
                
            }
        }
    }

    private bool TargetInViewRange(Vector3 targetPos, string targetTag){
        bool inAngle;
        bool inRange;
        bool correctTag;

        //calc angle
        Vector2 dir = targetPos - transform.position;
        float angleToTarget = Vector2.Angle(transform.up, dir);

        //raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, fieldOfView.viewDistance, raycastLayer);
        if(hit && hit.collider.tag == targetTag){
            correctTag = true; //this implies no obstacles were in the way
        }
        else{
            correctTag = false;
        }
        Debug.DrawRay(transform.position, dir, Color.red);

        inAngle = angleToTarget < fieldOfView.viewAngle / 2;
        inRange = Vector2.Distance(transform.position, targetPos) < fieldOfView.viewDistance;

        return correctTag && inAngle && inRange;
    }

    private void SearchForTarget() {
        if (movementController != null) {
            searchTimer += Time.fixedDeltaTime;
            if (searchTimer > searchTimerMax) {
                searchTimer = 0f;
                //search for any nearby targets in line of sight
                RaycastHit2D[] results = new RaycastHit2D[10];
                results = Physics2D.CircleCastAll(transform.position, detectionRange, Vector2.zero);

                if (results.Length > 0) {
                    //sort array by distance (The function already sorts by distance)
                    //check for player or orcs in range, and set target
                    for (int i = 0; i < results.Length; i++) {
                        if (results[i].collider.tag == "Orc") { //Orc takes priority over player
                            target = results[i].transform;
                            movementController.GetMoveCommand(target.position);
                            //Debug.Log("Moving towards target " + results[i].collider.tag);
                            return;
                        }
                    }
                    //check for player if no orcs. Will eventually require line of sight
                    //for (int i = 0; i < results.Length; i++) {
                    //    if (results[i].collider.tag == "Player") { //Orc takes priority over player
                    //        target = results[i].transform;
                    //        movementController.GetMoveCommand(target.position);
                    //        //Debug.Log("Moving towards target " + results[i].collider.tag);
                    //        return;
                    //    }
                    //}
                }
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.tag == "Orc") {
            //stun self
            stunned = true;
            //Debug.Log("stunned");
            //stop movement controller
            movementController.enabled = false;
            //apply impulse
            if(collision.contactCount > 0)
                rb.AddForce(collision.contacts[0].normal * 10f, ForceMode2D.Impulse);
        }
    }

    // private void OnTriggerEnter2D(Collider2D collision) {
    //     if (collision.tag == "Player" && lineOfSight) {
    //         //player entered into line of sight
    //         target = collision.transform;
    //         chasing = true; //focus on player
    //         playerInSight = true;
    //         playerHideTimer = 0f; //reset timer
    //         //chaseIndicator.sprite = chasingSprite;
    //     }
    //     else if(collision.tag == "Orc" && !chasing) {
    //         //orc is less priority
    //         target = collision.transform;
    //     }
    // }

    // private void OnTriggerExit2D(Collider2D collision) {
    //     if (collision.tag == "Player") {
    //         playerInSight = false;
    //         //chaseIndicator.sprite = lostSiteSprite;
    //     }
    // }
}
