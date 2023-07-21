using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    //public settings
    public float move_speed = 10f;
    public float spy_range = 10f; //how far out can you spy on the hero
    public float spy_timer_max = 1f; //how many seconds before incrementing spy meter
    public int health_max = 1; //how many hits the player can receive before dying
    public LayerMask raycastLayer;
    
    //private settings
    private Vector2 movement = Vector2.zero;
    private Vector2 facing = Vector2.up; //direction player is facing
    private float rotation_angle = 0f;
    [HideInInspector]
    public int spy_progress = 0; //100 is a full meter
    private float spy_timer = 0f; //current timer progress
    private int health_current;
    private bool lineOfSight; //no obstacles between player and hero
    

    //[SerializeField] private int max_slidervalue = 10;

    //components
    private Rigidbody2D rb;
    //private Collider2D spy_view;
    public Image spyMeterFillImage;
    public Transform heroT;
    //private GameObject FoVSprite;
    //For Spymeter
    //[SerializeField] private Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        //get components
        rb = GetComponent<Rigidbody2D>();
        //spy_view = rb.GetComponent<Collider2D>();

        health_current = health_max;

        //slider.maxValue = max_slidervalue;
        //slider.gameObject.SetActive(false);
        //slider.interactable = false;
    }

    // Update is called once per frame
    void Update() {
        movement = Vector2.zero; //reset every frame, probably not necessary
        //get input
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //facing = new Vector2(Input.GetAxisRaw("something"), Input.GetAxisRaw("something"));
        facing = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        rotation_angle = Vector2.SignedAngle(Vector2.up, facing);
        //clamp rotation to steps of 45 degrees
        //rotation_angle += 360; //make positive
        //rotation_angle += (int)(45 / 2); //back up half step
        //int wedge_number = (int)(rotation_angle / 45);
        //rotation_angle = wedge_number * 45;
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
        if (heroT) {
            Vector2 dir = heroT.position - transform.position;
            RaycastHit2D[] results = new RaycastHit2D[0];
            //int hit_count = Physics2D.Raycast(dir, results, Mathf.Infinity);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, raycastLayer);
            Debug.DrawRay(transform.position, dir, Color.red);

            if (hit.collider.tag == "Hero") {
                lineOfSight = true;
            }
            else {
                lineOfSight = false;
            }
            //if (hit_count == 1 && results[0].collider.tag == "Hero") {
            //    //only hero was hit, have a clear line of sight to hero
            //    lineOfSight = true;
            //}
            //else {
            //    lineOfSight = false;
            //}
        }
        
            //    //only hero was hit
            //    IncrementSpymeter();
            //}
            //else if (hit_count >= 2) {
            //    //see if hero was hit at all
            //    for (int i = 0; i < hit_count; i++) {
            //        if (results[i].collider.tag == "Hero") {
            //            //check that hero is closer to player than all other hits
            //            for (int j = 0; j < hit_count; j++) {
            //                if (j == i) continue; //skip hero index
            //                if ((results[j].transform.position - transform.position).magnitude < (results[i].transform.position - transform.position).magnitude) {
            //                    //player is closer to obstacle than hero
            //                    //so do nothing
            //                    return;
            //                }
            //            }
            //            IncrementSpymeter();
            //        }
            //    }
            //}
        }

    /// <summary>
    /// Called when player has line of sight to hero
    /// </summary>
    private void IncrementSpymeter() {
        if (!lineOfSight) {
            //not a clear shot
            //Debug.Log("Obstructed View");
            return;
        }
        //increment timer
        spy_timer += Time.fixedDeltaTime;
        if (spy_timer > spy_timer_max) {
            //reset timer
            spy_progress += 1;
            Debug.Log("Spy Meter: " + spy_progress);

            UpdateSpymeter(spy_progress); // Update the Spymeter

            spy_timer = 0f;

            if(spy_progress >= 100) {
                //win level
                Debug.Log("YOU WIN");
                FindObjectOfType<LocalMapManager>().LoadLevel();
            }

            //Update visuals
            //UpdateSpymeter()
        }
    }

    public void UpdateSpymeter( int spy_progress)
    {
        spyMeterFillImage.fillAmount = (float)spy_progress / 100f; //total

        //if(spy_progress == max_slidervalue)
        //{
        //    slider.value = 0;
        //    spy_progress = 0;
        //}
    }

    private void TakeDamage(int damageAmount) {
        health_current -= damageAmount;
        //show damage being taken
        if (health_current <= 0) {
            //die
            //be sent back to overworld
            //Spaghetti Code
            FindObjectOfType<LocalMapManager>().PlayerDied();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.collider.tag == "Arrow") {
            TakeDamage(1);
        }
        if(collision.collider.tag == "Hero") {
            TakeDamage(1);
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if(collision.tag == "Hero") {
            IncrementSpymeter();
        }
    }
}
