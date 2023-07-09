using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OrcCamp : MonoBehaviour
{
    private bool currentlyWorking = false; //are Orcs producing resources
    private float resourceTimer = 0f;
    public float resourceTimerMax = 1f;
    public int resourceCount = 0;
    public TMP_Text resourceCounterText;
    public GameObject orcArmy;
    private SpriteRenderer spriteRenderer;
    public Sprite workingSprite;
    public Sprite restingSprite;
    public int orcCountMin;
    public int orcCountMax;


    // Start is called before the first frame update
    void Start()
    {
        //get Components
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentlyWorking) {
            resourceTimer += Time.deltaTime;
            //add resources every second
            if (resourceTimer > resourceTimerMax) {
                //OverworldManager.Instance.AddResources(10);
                AddResources(10);
                resourceTimer = 0f;
            }
        }
    }

    public void AddResources(int amount) {
        //OverworldManager.Instance.Resources += amount;
        resourceCount += amount;
        if (resourceCount >= 100) {
            resourceCount -= 100;
            //spawn orc army
            GameObject o = Instantiate(orcArmy, transform.position, transform.rotation);
            o.GetComponent<OverworldNPCController>().orcCount = (int)Random.Range(orcCountMin, orcCountMax);
            currentlyWorking = false;
            UpdateSprite();
        }
        //update resource counter
        resourceCounterText.text = "Resources\n" + resourceCount + "/100";
        //play animation showing counter increasing
    }

    public void UpdateSprite() {
        if (currentlyWorking) {
            //use working sprite
            spriteRenderer.sprite = workingSprite;
        }
        else {
            //use resting sprite
            spriteRenderer.sprite = restingSprite;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.collider.tag == "Player") {
            currentlyWorking = true;
            UpdateSprite();
        }
    }
}
