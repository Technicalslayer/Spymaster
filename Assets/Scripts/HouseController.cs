using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseController : MonoBehaviour
{
    public int health;
    public int maxHealth = 5;
    public int minHealth = 2;

    private void Start() {
        //pick a random amount of health
        health = Mathf.RoundToInt(Random.Range(minHealth, maxHealth));
        Debug.Log("Health: " + health);
    }

    private void TakeDamage() {
        health -= 1;
        if (health <= 0) {
            //destroy house
            gameObject.SetActive(false);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.tag == "Orc") {
            //take damage
            TakeDamage();
        }
    }
}
