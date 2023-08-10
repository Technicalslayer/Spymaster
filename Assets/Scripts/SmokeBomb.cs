using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeBomb : MonoBehaviour
{
    private ParticleSystem smokeParticles;
    public float lifeSpan; //how long to stay active
    public float effectRange; //how far the confusion effect applies
    private float lifeSpanTimer;

    // Start is called before the first frame update
    void Start()
    {
        //get components
        smokeParticles = GetComponent<ParticleSystem>();
        GetComponent<CircleCollider2D>().radius = effectRange;
    }

    // Update is called once per frame
    void Update()
    {
        if(lifeSpanTimer > lifeSpan){
            //destroy
            smokeParticles.Stop();
            Destroy(gameObject);
        }
    }

    // private void FixedUpdate() {
    //     //check for hero being in range
    //     Collider2D[] c = Physics2D.OverlapCircleAll(transform.position, effectRange);
        
    //     for(int i = 0; i < c.Length; i++){
    //         if (c[i].tag == "Hero"){
    //             c[i].GetComponent<HeroController>().ApplyConfusion();
    //         }
    //     }
    // }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Hero"){
            other.GetComponent<Hero>().ApplyConfusion();
        }
    }
}
