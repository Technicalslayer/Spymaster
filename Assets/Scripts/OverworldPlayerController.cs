using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverworldPlayerController : MonoBehaviour
{

    public float move_speed = 10f;

    private Vector2 movement = Vector2.zero;
    private Vector2 facing = Vector2.up; //direction player is facing
    private float rotation_angle = 0f;

    //components
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start() {
        //get components
        rb = GetComponent<Rigidbody2D>();
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
        rotation_angle += (int)(45 / 2); //back up half step
        int wedge_number = (int)(rotation_angle / 45);
        rotation_angle = wedge_number * 45;
        //rotation_angle = rotation_angle - (rotation_angle % 45);
        //Debug.Log(rotation_angle);
    }

    private void FixedUpdate() {
        movement.Normalize();
        rb.velocity = movement * move_speed;
        rb.rotation = rotation_angle;
    }


    private void OnCollisionEnter2D(Collision2D collision) {
       
    }
}
