using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcCamp : MonoBehaviour
{
    private bool currentlyWorking; //are Orcs producing resources
    private float resourceTimer = 0f;
    public float resourceTimerMax = 1f;
    public int resourceCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentlyWorking) {
            //add resources every second
            if (resourceTimer > resourceTimerMax) {
                //OverworldManager.Instance.AddResources(10);
                AddResources(10);
            }
        }
    }

    public void AddResources(int amount) {
        //OverworldManager.Instance.Resources += amount;
        resourceCount += amount;
        //update resource counter
    }
}
