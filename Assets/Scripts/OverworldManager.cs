using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//should probably rename this
public class OverworldManager : MonoBehaviour
{
    public static OverworldManager Instance; //singleton instance, used between scenes

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int SpyProgress = 0; //tracks the player's spy meter progress in between levels

    public void ResetSpyProgress() {
        SpyProgress = 0;
        //update spy meter
    }
}
