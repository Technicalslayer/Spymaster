using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corviary : MonoBehaviour
{
    public GameObject goldCrow;
    public GameObject silverCrow;
    public GameObject bronzeCrow;
    public Transform crowTarget; //big bad location

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendReport() {
        int progressValue = OverworldManager.Instance.SpyProgress;
        if(progressValue == 0) {
            return; //no point in reporting
        }

        if (progressValue == 100) {
            //perfect, send gold crow
            //give player extra camp to place
            //track perfects
            Instantiate(goldCrow);
        }
        else if (progressValue >= 50) {
            //Great, send silver crow
            Instantiate(silverCrow);
        }
        else {
            //not enough, send bronze crow
            //track failures
            Instantiate(bronzeCrow);
        }
        OverworldManager.Instance.ResetSpyProgress();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        //should get the player's spymeter value
        if (collision.collider.tag == "Player") {
            //collision.gameObject.GetComponent<OverworldPlayerController>();
            SendReport();
        }
    }
}
