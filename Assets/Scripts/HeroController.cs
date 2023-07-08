using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    public float detectionRange = 100f; //how far to search for the player or orcs
    public float clashRange = 1f; //how close to begin charge
    public float shotRange = 10f; //how far away to stand from player when attacking
    public GameObject arrowPrefab;
    public LayerMask raycastTargetLayer;

    private float searchTimer = 0f;
    private float searchTimerMax = 0.5f; //time in seconds between searches for player/orcs
    private Transform target; //target to make way towards
    private float stunTimer = 0f;
    private float stunTimerMax = 0.5f;
    private bool stunned = false;
    private bool shotOnCooldown = false;
    private float shotTimerMax = 0.5f;
    private float shotTimer = 0f;

    private MovementController2D movementController;
    private Rigidbody2D rb;
    private Collider2D c;

    // Start is called before the first frame update
    void Start()
    {
        //get components
        movementController = GetComponent<MovementController2D>();
        rb = GetComponent<Rigidbody2D>();
        c = GetComponent<Collider2D>();
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

        if (shotOnCooldown) {
            shotTimer += Time.deltaTime;
            if (shotTimer > shotTimerMax) {
                shotOnCooldown = false;
                shotTimer = 0f;
            }
        }
    }

    private void FixedUpdate() {
        if (stunned) {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, stunTimer / stunTimerMax); //slow down slide //I don't think this does anything cuz of the MovePosition function
        }
        else {
            SearchForTarget();
            //rotate towards movement
            if (movementController.intendedVelocity != null) {
                rb.MoveRotation(Vector2.SignedAngle(Vector2.right, movementController.intendedVelocity));
            }
        }

        ShootTarget();
        
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
                    for (int i = 0; i < results.Length; i++) {
                        if (results[i].collider.tag == "Player") { //Orc takes priority over player
                            target = results[i].transform;
                            movementController.GetMoveCommand(target.position);
                            //Debug.Log("Moving towards target " + results[i].collider.tag);
                            return;
                        }
                    }
                }
            }
        }
    }

    private void ShootTarget() {
        //check if in range to shoot at player
        //stop moving and focus fire, only moving if out of range or losing line of sight
        //first check line of sight
        if (target != null && target.tag == "Player") {

            //RaycastHit2D hit = Physics2D.Raycast(transform.position, target.position - transform.position);
            Vector2 dirToTarget = target.position - transform.position;
            dirToTarget.Normalize();
            RaycastHit2D[] hit = new RaycastHit2D[1];
            int hit_count = c.Raycast(dirToTarget, hit, Mathf.Infinity, raycastTargetLayer); //need to ignore arrow colliders

            //Debug.DrawRay(transform.position, target.position - transform.position, Color.red);
            //Debug.Log(hit.collider.name);

            for (int i = 0; i < hit_count; i++) {
                if (hit[i].collider.tag == "Player") {
                    Debug.Log("hitting player");
                    if (hit[i].distance <= shotRange && !shotOnCooldown) {
                        //start shooting
                        GameObject o = Instantiate(arrowPrefab, transform.position, Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, dirToTarget))); //temp
                        o.GetComponent<ArrowController>().velocity = dirToTarget;
                        Debug.Log("Shooting player");
                        shotOnCooldown = true;
                        movementController.enabled = false;
                        return;
                    }
                    else if (hit[i].distance >= shotRange) {
                        //move closer
                        movementController.enabled = true;
                        return;
                    }
                }
                else {
                    movementController.enabled = true; //keep moving, didn't see player
                }
            }
            //movementController.enabled = true;


        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.tag == "Orc") {
            //stun self
            stunned = true;
            Debug.Log("stunned");
            //stop movement controller
            movementController.enabled = false;
            //apply impulse
            if(collision.contactCount > 0)
                rb.AddForce(collision.contacts[0].normal * 10f, ForceMode2D.Impulse);
        }
    }
}
