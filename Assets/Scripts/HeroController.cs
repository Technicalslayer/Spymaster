using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    public float chaseRotateSpeed = 180f; //degrees per second? //how fast to turn when chasing
    public float wanderRotateSpeed = 100f; //how fast to turn when not chasing
    //public float detectionRange = 100f; //how far to search for the player or orcs
    public LayerMask raycastLayer;
    public List<Vector2> patrolPoints = new List<Vector2>();
    public Transform playerT;
    //public SpriteRenderer chaseIndicator; //sprite shows if hero is chasing player, has lost sight, or is wandering
    public Sprite chasingSprite;
    public Sprite lostSiteSprite;
    public Sprite wanderingSprite;

    //private float searchTimer = 0f;
    private float searchTimerMax = 15f; //how long to search for missing player
    private Transform target; //target to make way towards
    private float stunTimer = 0f;
    private float stunTimerMax = 0.7f;
    private bool stunned = false;
    private bool chasing = false; //indicates hero is chasing player
    private bool playerInSight = false;
    private bool lookingAround; //indicates hero should turn around randomly to locate player
    private float lookingAroundAngle; //random direction hero should turn towards
    private float wanderTimer = 0f;
    private float wanderTimeMax = 5f; //how long between selecting a new random point to travel to
    private int patrolPointIndex = 0; //current patrol target index?
    private float playerHideTimer = 0f; //how long has the player been out of sight?
    private float playerHideTimeMax = 7f; //how long for the player to be out of sight before searcing
    private Vector2 playerLastLocation; //last known location of the player
    private bool searchingForPlayer; //looking around the last known location of the player
    private GameObject[] orcs;
    private float currentRotateSpeed;


    private MovementController2D movementController;
    private Rigidbody2D rb;
    private Collider2D c;
    private FieldOfView fieldOfView;

    private IEnumerator CheckForOrcs(){
        
        while(true){
            if(OrcsPresent()){
                target = FindClosestOrc().transform;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator LookForPlayer(){
        float searchTimer = 0f;
        while(searchingForPlayer && chasing){
            
            //lost sight of player
            Debug.Log("Inside LookForPlayer while loop");
            
            //if distance between hero and last known position is small enough, then stop moving and look around
            if(Vector2.Distance(playerLastLocation, transform.position) < 0.5f){
                //look around. pick random angle to look at, turn towards it
                
                if(!lookingAround){
                    StartCoroutine(LookAround());
                }
            }
            else{
                //move towards last known sight
                movementController.GetMoveCommand(playerLastLocation);
            }

            yield return null;

            searchTimer += Time.deltaTime;
            if(searchTimer > searchTimerMax){
                searchingForPlayer = false;
                chasing = false;
                lookingAround = false;
                yield break; //exit coroutine
            }
        }
        lookingAround = false;
    }

    private IEnumerator LookAround(){
        lookingAround = true;
        while(lookingAround){
            //pick a random direction and turn that way
            lookingAroundAngle = Vector2.SignedAngle(Vector2.up, Random.insideUnitCircle);
            yield return new WaitForSeconds(1.5f + Random.Range(-0.5f, 1f));
        }
    }

    private IEnumerator ChaseTarget(){
        yield return null; //wait frame to allow movement controller to initialize.
        while(true){
            if (target != null) {
                //moving towards target
                movementController.GetMoveCommand(target.position);
            }
            else if(chasing && !searchingForPlayer){
                movementController.GetMoveCommand(playerT.position);
            }
            yield return new WaitForSeconds(0.3f);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        //get components
        movementController = GetComponent<MovementController2D>();
        rb = GetComponent<Rigidbody2D>();
        c = GetComponent<Collider2D>();
        fieldOfView = GetComponentInChildren<FieldOfView>();

        //start coroutines
        StartCoroutine(CheckForOrcs());
        StartCoroutine(ChaseTarget());

        currentRotateSpeed = wanderRotateSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(chasing || target != null){
            currentRotateSpeed = chaseRotateSpeed;
        }
        else{
            currentRotateSpeed = wanderRotateSpeed;
        }

        if(chasing){
            //chasing player
            if(playerInSight){
                //reset timer
                playerHideTimer = 0f;
                //save last known position
                playerLastLocation = playerT.position;
                Debug.Log("Can See Player");
            }
            else{
                Debug.Log("Can't See Player");
                //lost sight
                //start timer for player being out of sight
                playerHideTimer += Time.deltaTime;
                if(playerHideTimer > playerHideTimeMax){

                    //move towards last known location
                    //increment timer
                    if(!searchingForPlayer){
                        Debug.Log("Calling LookForPlayer");
                        searchingForPlayer = true;
                        StartCoroutine(LookForPlayer());
                    }
                }
            }
        }

        if(!chasing && target == null){ //no orcs and don't see player
            //wander. follow patrol points
            Debug.Log("Wandering");
            if(patrolPoints.Count > 0){
                //check distance to patrol point
                if(Vector2.Distance(transform.position, patrolPoints[patrolPointIndex]) < 0.5f){
                    //close, check timer and look around
                    wanderTimer += Time.deltaTime;
                    if(wanderTimer > wanderTimeMax){
                        patrolPointIndex++;
                        if(patrolPointIndex >= patrolPoints.Count){
                            patrolPointIndex = 0; //go back to start of patrol
                        }
                        wanderTimer = 0f;//reset timer
                        lookingAround = false;
                    }
                    else{
                        if(!lookingAround){
                            StartCoroutine(LookAround());
                        }
                    }
                    
                }
                else{
                    movementController.GetMoveCommand(patrolPoints[patrolPointIndex]);
                }
                
            }
            // else{
            //     //pick random points? Physics2D.OverlapPoint
            // }

        }
        //if target != null, chasing orc or repairing house
        //if stunned, just can't move, but nothing else changes


        
        if (stunned) {
            stunTimer += Time.deltaTime;
            if (stunTimer > stunTimerMax) {
                stunned = false;
                stunTimer = 0f;
                //enable movement controller
                movementController.enabled = true;
            }
        }

        // if (chasing) {
        //     //don't do anything other than chase player
        //     if (!playerInSight) {
        //         playerHideTimer += Time.deltaTime;
        //         if (playerHideTimer > playerHideTimeMax) {
        //             //lost player, resume normal behavior
        //             chasing = false;
        //             //chaseIndicator.sprite = wanderingSprite;
        //         }
        //     }
        // }
        // else {
        //     //check if target still exists
        //     wanderTimer += Time.deltaTime;
        //     if (target == null && wanderTimer >= wanderTimeMax) {
        //         wanderTimer = 0f;
        //         //find new target or start wandering


        //         //start wandering
        //         if (patrolPoints.Count > 0) {
        //             patrolPointIndex++;
        //             if (patrolPointIndex >= patrolPoints.Count) {
        //                 patrolPointIndex = 0; //reset patrol
        //             }
        //             movementController.GetMoveCommand(patrolPoints[patrolPointIndex]);
        //         }
        //     }
        // }
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
                float rotateStepSize = currentRotateSpeed * Time.fixedDeltaTime;
                //rb.MoveRotation(Mathf.LerpAngle(currentAngle, desiredAngle, 0.05f));
                if(Mathf.Abs(Mathf.DeltaAngle(currentAngle, desiredAngle)) > rotateStepSize && !lookingAround){
                    //move towards desired angle at set speed
                    rb.MoveRotation(Mathf.MoveTowardsAngle(currentAngle, desiredAngle, rotateStepSize));
                }
                else{
                    //too close for step, snap to angle
                    rb.MoveRotation(desiredAngle);
                }
            }
        }

        if(lookingAround){
            Debug.Log("Looking around in Fixed Update");
            float currentAngle = rb.rotation;
            float rotateStepSize = currentRotateSpeed * Time.fixedDeltaTime;
            //rb.MoveRotation(Mathf.LerpAngle(currentAngle, desiredAngle, 0.05f));
            if(Mathf.Abs(Mathf.DeltaAngle(currentAngle, lookingAroundAngle)) > rotateStepSize){
                //move towards desired angle at set speed
                rb.MoveRotation(Mathf.MoveTowardsAngle(currentAngle, lookingAroundAngle, rotateStepSize));
            }
            else{
                //too close for step, snap to angle
                rb.MoveRotation(lookingAroundAngle);
            }
        }

        

        //check for targets
        //RaycastHit2D[] targetsInViewDistance = Physics2D.CircleCastAll(transform.position, fieldOfView.viewDistance, Vector2.zero, 0f, raycastLayer);
        //check what targets are valid
        if (playerT && target == null) {
            if (TargetInViewRange(playerT.position, "Player")) {
                playerInSight = true;
                chasing = true;
                searchingForPlayer = false; //found player, no need to look around
                lookingAround = false;
            }
            else{
                //chasing player but can't see it
                //move towards last known location and look around
                playerInSight = false;
            }
        }
    }

    private bool OrcsPresent(){
        orcs = GameObject.FindGameObjectsWithTag("Orc");
        return orcs.Length > 0;
    }

    private GameObject FindClosestOrc(){
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in orcs)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
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
        //Debug.Log(correctTag + " " +  inAngle + " " + inRange);
        return correctTag && inAngle && inRange;
    }

    // private void SearchForTarget() {
    //     if (movementController != null) {
    //         searchTimer += Time.fixedDeltaTime;
    //         if (searchTimer > searchTimerMax) {
    //             searchTimer = 0f;
    //             //search for any nearby targets in line of sight
    //             RaycastHit2D[] results = new RaycastHit2D[10];
    //             results = Physics2D.CircleCastAll(transform.position, detectionRange, Vector2.zero);

    //             if (results.Length > 0) {
    //                 //sort array by distance (The function already sorts by distance)
    //                 //check for player or orcs in range, and set target
    //                 for (int i = 0; i < results.Length; i++) {
    //                     if (results[i].collider.tag == "Orc") { //Orc takes priority over player
    //                         target = results[i].transform;
    //                         movementController.GetMoveCommand(target.position);
    //                         //Debug.Log("Moving towards target " + results[i].collider.tag);
    //                         return;
    //                     }
    //                 }
    //                 //check for player if no orcs. Will eventually require line of sight
    //                 //for (int i = 0; i < results.Length; i++) {
    //                 //    if (results[i].collider.tag == "Player") { //Orc takes priority over player
    //                 //        target = results[i].transform;
    //                 //        movementController.GetMoveCommand(target.position);
    //                 //        //Debug.Log("Moving towards target " + results[i].collider.tag);
    //                 //        return;
    //                 //    }
    //                 //}
    //             }
    //         }
    //     }
    // }


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
