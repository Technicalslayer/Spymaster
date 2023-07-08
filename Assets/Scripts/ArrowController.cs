using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{

    private Rigidbody2D rb;
    private float lifeSpan = 0f;
    public float maxLifeSpan = 3f;
    public float moveSpeed = 10f;
    public Vector2 velocity = Vector2.up;
    // Start is called before the first frame update
    void Start()
    {
        //get components
        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(lifeSpan >= maxLifeSpan) {
            //destroy
            Destroy(gameObject);
        }
        lifeSpan += Time.deltaTime;
        //rb.velocity = rb.GetVector(velocity) * moveSpeed;
        rb.velocity = velocity * moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Destroy(gameObject);
    }
}
