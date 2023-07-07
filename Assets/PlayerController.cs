using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float move_speed = 10f;
    public float spy_range = 10f; //how far out can you spy on the hero
    public float spy_timer_max = 1f; //how many seconds before incrementing spy meter

    private Vector2 movement = Vector2.zero;
    private Vector2 facing = Vector2.up; //direction player is facing
    private float rotation_angle = 0f;
    private int spy_progress = 0; //100 is a full meter
    private float spy_timer = 0f; //current timer progress

    //components
    private Rigidbody2D rb;
    private Collider2D spy_view;

    // Start is called before the first frame update
    void Start()
    {
        //get components
        rb = GetComponent<Rigidbody2D>();
        spy_view = rb.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update() {
        movement = Vector2.zero; //reset every frame, probably not necessary
        //get input
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //facing = new Vector2(Input.GetAxisRaw("something"), Input.GetAxisRaw("something"));
        facing = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        rotation_angle = Vector2.SignedAngle(Vector2.right, facing) - 90f;
        //clamp rotation to steps of 45 degrees
        rotation_angle += 360; //make positive
        rotation_angle += 45 / 2; //back up half step
        int wedge_number = (int)(rotation_angle / 45);
        rotation_angle = wedge_number * 45;
        //rotation_angle = rotation_angle - (rotation_angle % 45);
        //Debug.Log(rotation_angle);
    }

    private void FixedUpdate() {
        movement.Normalize();
        rb.velocity = movement * move_speed;
        rb.rotation = rotation_angle;

        //shapecast for spy FoV
        //cast, hitting hero and obstacle layers
        //if hit obstacle, ignore cast
        //if hit only hero or hero hit before obstacle, then count as success, start to fill up meter
        RaycastHit2D[] results = new RaycastHit2D[5];
        int hit_count = spy_view.Cast(transform.up, results, spy_range);
        if (hit_count == 1 && results[0].collider.tag == "hero") {
            //only hero was hit
            IncrementSpymeter();
        }
        else if (hit_count >= 2) {
            //see if hero was hit at all
            for (int i = 0; i < results.Length; i++) {
                if (results[i].collider.tag == "hero") {
                    //check that hero is closer to player than all other hits
                    for (int j = 0; j < results.Length; j++) {
                        if (j == i) continue; //skip hero index
                        if ((results[j].collider.transform.position - transform.position).magnitude < (results[i].collider.transform.position - transform.position).magnitude) {
                            //player is closer to obstacle than hero
                            //so do nothing
                            return;
                        }
                    }
                    IncrementSpymeter();
                }
            }
        }
    }

    /// <summary>
    /// Called when player has line of sight to hero
    /// </summary>
    private void IncrementSpymeter() {
        //increment timer
        spy_timer += Time.fixedDeltaTime;
        if (spy_timer > spy_timer_max) {
            //reset timer
            spy_progress += 1;
            Debug.Log(spy_progress);
            spy_timer = 0f;
            //Update visuals
            //UpdateSpymeter()
        }
    }
}
