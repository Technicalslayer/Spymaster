using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Finds all active orcs and calls them towards the waypoint. Destroys self after some time.
/// </summary>
public class Waypoint : MonoBehaviour
{
    public float lifeSpan = 10f; //how long to exist
    private float lifeTimer;


    private IEnumerator CallOrcs() {
        while (true) {
            Orc[] orcs = FindObjectsByType<Orc>(FindObjectsSortMode.None);
            foreach (Orc orc in orcs) {
                orc.AssignWaypoint(this);
                yield return new WaitForSeconds(Random.Range(0, 0.2f)); //have them called in staggered way?
            }

            yield return new WaitForSeconds(1f); //keep checking
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //find all orcs
        StartCoroutine(CallOrcs());
    }

    // Update is called once per frame
    void Update()
    {
        lifeTimer += Time.deltaTime;
        if(lifeTimer > lifeSpan) {
            Destroy(gameObject);
        }
    }
}
