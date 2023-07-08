using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    public float detectionRange = 10f; //how far to search for the player or orcs
    public float clashRange = 1f; //how close to begin charge

    private float searchTimer = 0f;
    private float searchTimerMax = 1f; //time in seconds between searches for player/orcs
    private Transform target; //target to make way towards
    private float stunTimer = 0f;
    private float stunTimerMax = 0.5f;
    private bool stunned = false;

    private MovementController2D movementController;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        //get components
        movementController = GetComponent<MovementController2D>();
        rb = GetComponent<Rigidbody2D>();
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
    }

    private void FixedUpdate() {
        if (stunned) {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, stunTimer / stunTimerMax); //slow down slide
        }
        else 
            SearchForTarget();
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
                            Debug.Log("Moving towards target " + results[i].collider.tag);
                            return;
                        }
                    }
                    //check for player if no orcs. Will eventually require line of sight
                    for (int i = 0; i < results.Length; i++) {
                        if (results[i].collider.tag == "Player") { //Orc takes priority over player
                            target = results[i].transform;
                            movementController.GetMoveCommand(target.position);
                            Debug.Log("Moving towards target " + results[i].collider.tag);
                            return;
                        }
                    }
                }
            }
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
