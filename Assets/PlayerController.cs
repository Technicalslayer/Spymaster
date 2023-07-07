using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float move_speed = 10f;

    private Vector2 movement = Vector2.zero;

    //components
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        //get components
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        movement = Vector2.zero;
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void FixedUpdate() {
        movement.Normalize();
        rb.velocity = movement * move_speed;
    }
}
