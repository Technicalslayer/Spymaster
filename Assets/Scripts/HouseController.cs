using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HouseController : MonoBehaviour
{
    public int health;
    public int maxHealth;
    public int minHealth = 2;
    public int id; //unique for each house in a scene
    public Sprite destroyedSprite;
    public Sprite damagedSprite;
    public Sprite goodSprite;
    private SpriteRenderer spriteRenderer;
    public bool destroyed = false;
    private bool takingDamage = false; //indicates house was just damaged, has invulnerability and plays animation


    private IEnumerator FallApart(){
        yield return null;
    }

    private IEnumerator ShowDamage(){
        Vector3 startPos = transform.position;
        //shake and flash
        for(int i = 0; i < 15; i++){
            transform.position += new Vector3(Mathf.Sin(Time.time * 10f) * 0.05f, 0, 0);
            spriteRenderer.color = i % 2 == 1 ? Color.white : Color.red;
            yield return new WaitForSeconds(0.1f);
        }
        takingDamage = false;
        transform.position = startPos;
        spriteRenderer.color = Color.white;
    }


    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();

        //pick a random amount of health
        health = Mathf.RoundToInt(Random.Range(minHealth, maxHealth));
        //Debug.Log("Health: " + health);
    }


    private void TakeDamage() {
        StartCoroutine(ShowDamage());
        health -= 1;
        spriteRenderer.sprite = damagedSprite;
        takingDamage = true;
        if (health <= 0) {
            //destroy house
            health = 0;
            spriteRenderer.sprite = destroyedSprite;
            destroyed = true;
            //play particles
            //StartCoroutine(FallApart());
            //gameObject.SetActive(false);
            //check if all houses are destroyed
            //FindObjectOfType<LocalMapManager>().CheckVillageStatus();
        }
    }

    
    private void RepairDamage(){
        health += 1;
        spriteRenderer.sprite = damagedSprite;
        destroyed = false;
        if (health >= maxHealth){
            health = maxHealth;
            spriteRenderer.sprite = goodSprite;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision) {
        if(takingDamage){
            return;
        }
        if (!destroyed && collision.collider.tag == "Orc") {
            //take damage
            TakeDamage();
        }
        else if(collision.collider.tag == "Hero"){
            //repair
            RepairDamage();
        }
    }
}
