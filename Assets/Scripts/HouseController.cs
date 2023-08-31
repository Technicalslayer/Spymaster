using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public GameObject smokeBombPickup;

    public AudioSource fallApartSoundSource;
    private Animator animator;
    public Vector2 pathfinderHelperOffset = Vector2.zero;


    private IEnumerator FallApart(){
        if (!destroyed) {
            destroyed = true;
            spriteRenderer.sprite = destroyedSprite;
            fallApartSoundSource.Play();
            yield return null;
            //spawn bomb pickup
            Instantiate(smokeBombPickup, transform.position + (Vector3)Random.insideUnitCircle * 2.0f, transform.rotation);
        }
    }

    //private IEnumerator ShowDamage(){
    //    Vector3 startPos = transform.position;
    //    //play sound

    //    //shake and flash
    //    for(int i = 0; i < 15; i++){
    //        transform.position += new Vector3(Mathf.Sin(Time.time * 10f) * 0.05f, 0, 0);
    //        spriteRenderer.color = i % 2 == 1 ? Color.white : Color.red;
    //        yield return new WaitForSeconds(0.1f);
    //    }
    //    takingDamage = false;
    //    transform.position = startPos;
    //    spriteRenderer.color = Color.white;
    //}


    private void Start() {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();

        //pick a random amount of health
        maxHealth = Mathf.RoundToInt(Random.Range(minHealth, maxHealth));
        //set health to max
        health = maxHealth;
        //Debug.Log("Health: " + health);

        if(pathfinderHelperOffset == Vector2.zero) {
            Debug.LogWarning("Is the pathfinder offset supposed to be 0,0?");
        }
    }


    public void TakeDamage() {
        animator.SetTrigger("damaged");
        health -= 1;
        spriteRenderer.sprite = damagedSprite;
        takingDamage = true;
        if (health <= 0) {
            //destroy house
            health = 0;
            
            //play particles
            StartCoroutine(FallApart());
            //gameObject.SetActive(false);
            //check if all houses are destroyed
            //FindObjectOfType<LocalMapManager>().CheckVillageStatus();
        }
    }

    
    public void RepairDamage(){
        animator.SetTrigger("repaired");
        health += 1;
        spriteRenderer.sprite = damagedSprite;
        destroyed = false;
        if (health >= maxHealth){
            health = maxHealth;
            spriteRenderer.sprite = goodSprite;
        }
        //TODO: play sound
        
    }


    private void OnCollisionEnter2D(Collision2D collision) {
        //if(takingDamage || destroyed){
        //    return;
        //}
        //if (collision.collider.tag == "Orc") {
        //    //take damage
        //    TakeDamage();
        //}
        //else if(collision.collider.tag == "Hero"){
        //    //repair
        //    RepairDamage();
        //}
    }
}
