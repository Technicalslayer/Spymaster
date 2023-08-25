using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Variables
    //public settings
    public float move_speed = 10f;
    //public int rotationDelayFramesMax = 5; //how many frames to prevent changing direction
    public float spy_timer_max = 1f; //how many seconds before incrementing spy meter
    public int health_max = 1; //how many hits the player can receive before dying
    public LayerMask raycastLayer;
    public int smokeBombCount = 0;
    public float waypointCooldownTime = 5f;

    
    //private settings
    private Vector2 movement = Vector2.zero;
    private Vector2 facing = Vector2.up; //direction player is facing
    private float rotation_angle = 0f;
    [HideInInspector]
    public float spy_progress = 0f; //100 is a full meter
    private float spy_timer = 0f; //current timer progress
    private int health_current;
    //private bool rotationDelay = false; //used to prevent snapping to the wrong direction when releasing input
    //private int rotationDelayFrames = 0;
    private bool waypointOnCooldown = false;
    private bool canSpy = true;
    #endregion

    #region Components
    private Rigidbody2D rb;
    public Image spyMeterFillImage;
    public TMP_Text spyMeterText1;
    public TMP_Text spyMeterText2;
    public Image waypointFillImage;
    public Image waypointColoredImage;
    public Image smokebombImage1;
    public Image smokebombImage2;
    public Image smokebombImage3;
    private Transform heroT;
    private FieldOfView fieldOfView;
    public GameObject smokeBombPrefab;
    public GameObject waypointPrefab;
    #endregion

    private IEnumerator WaypointCooldown() {
        waypointOnCooldown = true;
        float waypointTimer = 0f;
        waypointColoredImage.enabled = false;
        while(waypointTimer < waypointCooldownTime) {
            //show cooldown effect on HUD
            waypointFillImage.fillAmount = waypointTimer / waypointCooldownTime;
            yield return null;
            waypointTimer += Time.deltaTime;
        }
        
        waypointOnCooldown = false;

        waypointColoredImage.enabled = true;
        waypointFillImage.fillAmount = 0f;
    }
    // Start is called before the first frame update
    void Start()
    {
        //get components
        rb = GetComponent<Rigidbody2D>();
        fieldOfView = GetComponentInChildren<FieldOfView>();
        health_current = health_max;

        //find hero object transform
        heroT = FindObjectOfType<Hero>().transform;

        //check that I assigned everything
        if (spyMeterText1 == null) Debug.LogWarning("SpyMeterText1 hasn't been assigned!");
        if (spyMeterText2 == null) Debug.LogWarning("SpyMeterText2 hasn't been assigned!");
        if (spyMeterFillImage == null) Debug.LogWarning("SpyMeterFillImage hasn't been assigned!");
        if (waypointFillImage == null) Debug.LogWarning("waypointFillImage hasn't been assigned!");
        if (waypointColoredImage == null) Debug.LogWarning("waypointColoredImage hasn't been assigned!");
        if (smokebombImage1 == null) Debug.LogWarning("smokebombImage1 hasn't been assigned!");
        if (smokebombImage2 == null) Debug.LogWarning("smokebombImage2 hasn't been assigned!");
        if (smokebombImage3 == null) Debug.LogWarning("smokebombImage3 hasn't been assigned!");
    }

    // Update is called once per frame
    void Update() {
        GetInput();
    }

    private void FixedUpdate() {
        movement.Normalize();
        rb.velocity = movement * move_speed;
        rb.rotation = rotation_angle;

        //if hit only hero or hero hit before obstacle, then count as success, start to fill up meter
        if (heroT) {
            if (TargetInViewRange(heroT.position, "Hero") && canSpy) {
                IncrementSpymeter();
            }
        }
    }


    private void GetInput(){
        //get input
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //if(!rotationDelay && (Input.GetButtonUp("Horizontal") || Input.GetButtonUp("Vertical"))){
        //    //start timer to ignore key releases on next couple frames?
        //    rotationDelay = true;
        //}

        //if(rotationDelay){
        //    //do not read movement input for rotation for a few frames to prevent snapping to wrong direction
        //    rotationDelayFrames++;
        //    if(rotationDelayFrames >= rotationDelayFramesMax){
        //        rotationDelay = false;
        //        rotationDelayFrames = 0;
        //    }
        //}
        //else if(movement.magnitude > 0){
        //    facing = movement;
        //}
        //facing = new Vector2(Input.GetAxisRaw("something"), Input.GetAxisRaw("something"));
        facing = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        // if(movement.magnitude > 0)
        //     facing = movement;
        rotation_angle = Vector2.SignedAngle(Vector2.up, facing);

        if(Input.GetButtonDown("Jump") && smokeBombCount > 0){
            //spawn smoke bomb
            RemoveSmokebombImage();
        }
        if (Input.GetButtonDown("Fire") && !waypointOnCooldown) {
            Instantiate(waypointPrefab, transform.position, transform.rotation);
            StartCoroutine(WaypointCooldown());
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
            float multiplier = 1f - Vector2.Distance(transform.position, heroT.position) / fieldOfView.viewDistance; //Moves it within the domain of (0,1) and flips the values
            multiplier *= 2f;
            multiplier = Mathf.Clamp(multiplier, 0.1f, 1.5f);

            //multiplier = -Mathf.Pow(multiplier, 2f); //negative square to get a downward curve
            //multiplier = (multiplier * 2f) + 2f; //times 2 to stretch vertically, allows values 0 to 2. Add 2 to move it out of negatives
            spy_progress += 1f * multiplier; //should give more points when x is closer to 0. Double points at 0.
            //Debug.Log("Multiplier: " + multiplier);
            ////if close enough, add another point
            //if(Vector2.Distance(transform.position, heroT.position) < 3f) {
            //    spy_progress += 1;
            //    Debug.Log("Extra spy point: " +  spy_progress);
            //}
            //Debug.Log("Spy Meter: " + spy_progress);

            UpdateSpymeter((int)spy_progress); // Update the Spymeter

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
        spyMeterText1.text = spy_progress + "/100";
        spyMeterText2.text = spy_progress + "/100";
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

    public void RemoveSmokebombImage() {
        if (smokeBombCount > 0) {
            switch (smokeBombCount) {
                case 0:
                    break;
                case 1:
                    smokebombImage1.enabled = false;
                    break;
                case 2:
                    smokebombImage2.enabled = false;
                    break;
                case 3:
                    smokebombImage3.enabled = false;
                    break;
                default:
                    break;
            }
            smokeBombCount--;
            Instantiate(smokeBombPrefab, transform.position, transform.rotation);
        }
    }

    public void AddSmokebombImage() {
        if (smokeBombCount < 3) {
            smokeBombCount++;
            switch(smokeBombCount) {
                case 0:
                    break;
                case 1:
                    smokebombImage1.enabled = true;
                    break;
                case 2:
                    smokebombImage2.enabled = true;
                    break;
                case 3:
                    smokebombImage3.enabled = true;
                    break;
                default:
                    break;
            }
        }
    }

    public void DisableFoV() {
        fieldOfView.gameObject.SetActive(false);
        canSpy = false;
    }

    public void EnableFoV() {
        fieldOfView.gameObject.SetActive(true); 
        canSpy = true;
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
