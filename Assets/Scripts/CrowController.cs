using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float lifeSpan = 10f;
    private float lifeSpanTimer = 0f;
    public float bounce = 0.5f; //value that adds vertical randomness to horizontal flight

    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lifeSpanTimer += Time.deltaTime;
        if(lifeSpanTimer >= lifeSpan) {
            Destroy(gameObject);
        }

        transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        transform.position += Vector3.up * (Mathf.Sin(lifeSpanTimer * bounce)) * Time.deltaTime;
    }
}
