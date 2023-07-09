using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcController : MonoBehaviour
{
    public int health;
    public int maxHealth = 5;
    public int minHealth = 2;

    public float detectionRange = 10f; //how far to search for the player or orcs
    public float clashRange = 1f; //how close to begin charge
    public LayerMask circleCastLayer;
    private float searchTimer = 0f;
    private float searchTimerMax = 1f; //time in seconds between searches for player/orcs
    private Transform target; //target to make way towards
    private float stunTimer = 0f;
    private float stunTimerMax = 0.5f;
    private bool stunned = false;

    private MovementController2D movementController;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start() {
        //get components
        movementController = GetComponent<MovementController2D>();
        rb = GetComponent<Rigidbody2D>();

        //pick a random amount of health
        health = Mathf.RoundToInt(Random.Range(minHealth, maxHealth));
        Debug.Log("Health: " + health);
        //SearchForTarget();
    }

    // Update is called once per frame
    void Update() {
        if(target == null) {
            SearchForTarget(); //hacky solution cuz searching wasn't working on the first frame
        }
        else {
            searchTimer += Time.fixedDeltaTime;
            if (searchTimer > searchTimerMax) {
                searchTimer = 0f;
                movementController.GetMoveCommand(target.position);
            }
        }
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
        //else
            //SearchForTarget();
    }

    private void SearchForTarget() {
        if (movementController != null) {
            searchTimer += Time.fixedDeltaTime;
            //if (searchTimer > searchTimerMax) {
              if (true) { //I'm sorry
                searchTimer = 0f;
                //search for any nearby targets in line of sight
                //RaycastHit2D[] results = new RaycastHit2D[10];
                //results = Physics2D.CircleCastAll(transform.position, detectionRange, Vector2.zero, 0f, circleCastLayer);
                HouseController[] houses = FindObjectsByType<HouseController>(FindObjectsSortMode.None);

                //if (results.Length > 0) {
                //    //sort array by distance (The function already sorts by distance)
                //    //check for player or orcs in range, and set target
                //    for (int i = 0; i < results.Length; i++) {
                //        if (results[i].collider.tag == "House") {
                //            target = results[i].transform;
                //            movementController.GetMoveCommand(target.position);
                //            //Debug.Log("Moving towards target " + results[i].collider.tag);
                //            return;
                //        }
                //        //if (results[i].collider.tag == "Hero" && Vector2.Distance(results[i].collider.transform.position, transform.position) < clashRange) {
                //        if (results[i].collider.tag == "Hero") { 
                //            //hero is closest, focus on it
                //            target = results[i].transform;
                //            movementController.GetMoveCommand(target.position);
                //            //Debug.Log("Moving towards target " + results[i].collider.tag);
                //            return;
                //        }
                //    }
                //}
                //gonna ignore the hero for now
                //pick random house, focus on just it, nothing else matters
                if (houses.Length > 0) {
                    //pick random int
                    int rIndex = Random.Range(0, houses.Length);
                    Debug.Log(rIndex);
                    //target = houses[rIndex].gameObject.transform;
                    target = houses[rIndex].gameObject.transform;
                    movementController.GetMoveCommand(target.position);
                    Debug.Log(name + " is moving towards target " + houses[rIndex].name);
                }
            }
        }
    }

    private void TakeDamage() {
        health -= 1;
        if (health <= 0) {
            //destroy orc
            Destroy(gameObject);
            //update local status
            //FindObjectOfType<LocalMapManager>().CheckVillageStatus();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.tag == "Hero" || collision.collider.tag == "House") {
            //stun self
            stunned = true;
            //Debug.Log("stunned");
            //stop movement controller
            movementController.enabled = false;
            //apply impulse
            if (collision.contactCount > 0)
                rb.AddForce(collision.contacts[0].normal * 10f, ForceMode2D.Impulse);

            if(collision.collider.tag == "Hero") {
                TakeDamage();
            }
        }
    }
}
