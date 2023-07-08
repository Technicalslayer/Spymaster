using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    public float detectionRange = 10f; //how far to search for the player or orcs

    private float searchTimer = 0f;
    private float searchTimerMax = 2f; //time in seconds between searches for player/orcs
    private Transform target; //target to make way towards

    private MovementController2D movementController;

    // Start is called before the first frame update
    void Start()
    {
        //get components
        movementController = GetComponent<MovementController2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate() {
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
                    //check for player if no orcs
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
}
