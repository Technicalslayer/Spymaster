using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverworldPlayerController : MonoBehaviour
{

    //public settings
    public float move_speed = 10f;
    public int rotationDelayFramesMax = 5; //how many frames to prevent changing direction


    //private settings
    private Vector2 movement = Vector2.zero;
    private Vector2 facing = Vector2.up; //direction player is facing
    private float rotation_angle = 0f;
    private bool rotationDelay = false; //used to prevent snapping to the wrong direction when releasing input
    private int rotationDelayFrames = 0;


    //components
    private Rigidbody2D rb;
    public Image spyMeterFillImage;

    // Start is called before the first frame update
    void Start() {
        //get components
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        GetInput();
    }

    private void FixedUpdate() {
        movement.Normalize();
        rb.velocity = movement * move_speed;
        rb.rotation = rotation_angle;
    }


    private void GetInput() {
        //get input
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (!rotationDelay && (Input.GetButtonUp("Horizontal") || Input.GetButtonUp("Vertical"))) {
            //start timer to ignore key releases on next couple frames?
            rotationDelay = true;
        }

        if (rotationDelay) {
            //do not read movement input for rotation for a few frames to prevent snapping to wrong direction
            rotationDelayFrames++;
            if (rotationDelayFrames >= rotationDelayFramesMax) {
                rotationDelay = false;
                rotationDelayFrames = 0;
            }
        }
        else if (movement.magnitude > 0) {
            facing = movement;
        }
        rotation_angle = Vector2.SignedAngle(Vector2.up, facing);
    }

    public void UpdateSpymeter(int spy_progress) {
        spyMeterFillImage.fillAmount = (float)spy_progress / 100f; //total
    }


    private void OnCollisionEnter2D(Collision2D collision) {
        
    }

}
