using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HouseController : MonoBehaviour
{
    public int health;
    public int maxHealth;
    //public int minHealth = 2;
    public int id; //unique for each house in a scene

    private void Start() {
        ////pick a random amount of health
        //health = Mathf.RoundToInt(Random.Range(minHealth, maxHealth));
        //Debug.Log("Health: " + health);
    }

    //private void TakeDamage() {
    //    health -= 1;
    //    if (health <= 0) {
    //        //destroy house
    //        gameObject.SetActive(false);
    //        //check if all houses are destroyed
    //        //FindObjectOfType<LocalMapManager>().CheckVillageStatus();
    //    }
    //}
    //private void OnCollisionEnter2D(Collision2D collision) {
    //    if (collision.collider.tag == "Orc") {
    //        //take damage
    //        TakeDamage();
    //    }
    //}
}
