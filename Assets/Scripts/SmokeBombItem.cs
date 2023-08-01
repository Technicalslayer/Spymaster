using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeBombItem : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.collider.tag == "Player"){
            //add to player smoke bomb count
            other.collider.GetComponent<PlayerController>().smokeBombCount++;
            Destroy(gameObject);
        }
    }
}
