using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldNPCController : MonoBehaviour
{
    public float detectionRange = 100f; //how far to search for targets
    public LayerMask raycastTargetLayer;
    //public string targetTag = "Village";
    public bool isOrc = false; //temp
    public int orcCount = 0;

    private float searchTimer = 0f;
    private float searchTimerMax = 0.5f; //time in seconds between searches for player/orcs
    private Transform target; //target to make way towards

    private MovementController2D movementController;
    private Rigidbody2D rb;
    private Collider2D c;

    // Start is called before the first frame update
    void Start() {
        //get components
        movementController = GetComponent<MovementController2D>();
        rb = GetComponent<Rigidbody2D>();
        c = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void FixedUpdate() {
            SearchForTarget();
            //rotate towards movement
            if (movementController.intendedVelocity != null) {
                rb.MoveRotation(Vector2.SignedAngle(Vector2.right, movementController.intendedVelocity));
            }
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
                    for (int i = 0; i < results.Length; i++) {
                        if (results[i].collider.tag == "Village") {
                            if (isOrc) {
                                //check if village is vulnerable to orcs
                                if (results[i].collider.GetComponent<Village>().villageStatus == Village.VillageState.DESTROYED) {
                                    //can't attack
                                    continue;// check next village
                                }
                            }
                            else {
                                //check if village needs defending
                                if (results[i].collider.GetComponent<Village>().villageStatus != Village.VillageState.UNDER_ATTACK) {
                                    //can't defend
                                    continue;// check next village
                                }
                            }

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


    //private void OnCollisionEnter2D(Collision2D collision) {
    //    if (collision.collider.tag == "Village") {
    //        //enter village level
    //        //village will handle the logic here, need to know if village is under attack and if 
    //    }
    //}
}
