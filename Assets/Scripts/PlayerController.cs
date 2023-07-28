using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    //public settings
    public float move_speed = 10f;
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

    //[SerializeField] private int max_slidervalue = 10;

    //components
    private Rigidbody2D rb;
    public Image spyMeterFillImage;
    public Transform heroT;
    private FieldOfView fieldOfView;

    // Start is called before the first frame update
    void Start()
    {
        //get components
        rb = GetComponent<Rigidbody2D>();
        fieldOfView = GetComponentInChildren<FieldOfView>();
        health_current = health_max;
    }

    // Update is called once per frame
    void Update() {
        movement = Vector2.zero; //reset every frame, probably not necessary
        //get input
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //facing = new Vector2(Input.GetAxisRaw("something"), Input.GetAxisRaw("something"));
        facing = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        // if(movement.magnitude > 0)
        //     facing = movement;
        rotation_angle = Vector2.SignedAngle(Vector2.up, facing);
    }

    private void FixedUpdate() {
        movement.Normalize();
        rb.velocity = movement * move_speed;
        rb.rotation = rotation_angle;

        //if hit only hero or hero hit before obstacle, then count as success, start to fill up meter
        if (heroT) {
            if (TargetInViewRange(heroT.position, "Hero")) {
                IncrementSpymeter();
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
        //Debug.DrawRay(transform.position, dir, Color.red);

        inAngle = angleToTarget < fieldOfView.viewAngle / 2;
        inRange = Vector2.Distance(transform.position, targetPos) < fieldOfView.viewDistance;

        return correctTag && inAngle && inRange;
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
            //Debug.Log("Spy Meter: " + spy_progress);

            UpdateSpymeter(spy_progress); // Update the Spymeter

            spy_timer = 0f;

            if(spy_progress >= 100) {
                //win level
                Debug.Log("YOU WIN");
                FindObjectOfType<LocalMapManager>().LoadLevel();
            }
        }
    }

    public void UpdateSpymeter( int spy_progress)
    {
        spyMeterFillImage.fillAmount = (float)spy_progress / 100f; //total
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

    // private void OnTriggerStay2D(Collider2D collision) {
    //     if(collision.tag == "Hero") {
    //         IncrementSpymeter();
    //     }
    // }
}
