using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corviary : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        //should get the player's spymeter value
        if (collision.collider.tag == "Player") {
            collision.gameObject.GetComponent<OverworldPlayerController>();
        }
    }
}
