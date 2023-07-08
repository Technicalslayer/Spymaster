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

    public int SpyProgress = 0; //tracks the player's spy meter progress in between levels, updated when leaving level or sending report
    //public int Resources = 0; //tracks amount of resources player has collected
    public List<GameObject> overworldObjects = new List<GameObject>(); //list of objects to enable/disable when entering/leaving overworld map

    public void ResetSpyProgress() {
        SpyProgress = 0;
        //update spy meter
    }

    //public void AddResources(int amount) {
    //    Resources += amount;
    //    if
    //    UpdateResourceCounter();
    //}

    ///// <summary>
    ///// Updates display
    ///// </summary>
    //public void UpdateResourceCounter() {
    //    return;
    //}
}
