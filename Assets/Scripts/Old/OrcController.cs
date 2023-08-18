using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OrcController : MonoBehaviour
{
    public int health;
    public int maxHealth = 5;
    public int minHealth = 2;

    public float detectionRange = 10f; //how far to search for the player or orcs
    public float clashRange = 1f; //how close to begin charge
    public LayerMask circleCastLayer;
    public TMP_Text hpText;
    private float searchTimer = 0f;
    private float searchTimerMax = 1f; //time in seconds between searches for player/orcs
    private Transform target; //target to make way towards
    private float stunTimer = 0f;
    private float stunTimerMax = 0.7f;
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
        maxHealth = health;
        //Debug.Log("Health: " + health);
        //update HP text
        UpdateHealthText();
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
                //remove any destroyed houses from list
                List<HouseController> goodHouses = new List<HouseController>();
                foreach(HouseController h in houses){
                    if(!h.destroyed){
                        goodHouses.Add(h);
                    }
                }

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
                if (goodHouses.Count > 0) {
                    //pick random int
                    int rIndex = Random.Range(0, goodHouses.Count);
                    //Debug.Log(rIndex);
                    //target = houses[rIndex].gameObject.transform;
                    target = goodHouses[rIndex].gameObject.transform;
                    movementController.GetMoveCommand(target.position);
                    //Debug.Log(name + " is moving towards target " + houses[rIndex].name);
                }
                // else{
                //     //no targets, so chase hero
                //     target = FindObjectOfType<HeroController>().transform;
                // }
            }
        }
    }

    private void TakeDamage() {
        health -= 1;
        UpdateHealthText();
        if (health <= 0) {
            //destroy orc
            Destroy(gameObject);
            //update local status
            //FindObjectOfType<LocalMapManager>().CheckVillageStatus();
        }
        
    }

    public void UpdateHealthText() {
        hpText.text = "Orc\n" + health + "\\" + maxHealth;
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
            else if(collision.collider.tag == "House"){
                //see if house is still a valid target
                if(collision.collider.GetComponent<HouseController>().destroyed){
                    //find new hosue
                    SearchForTarget();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Hero") {
            //target hero until death lol
            target = collision.transform;
        }
    }
}
