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
    //public List<GameObject> overworldObjects = new List<GameObject>(); //list of objects to enable/disable when entering/leaving overworld map
    //public List<string> villageJSON = new List<string>(); //list of village data
    public List<LocalMapState> VillageStates = new List<LocalMapState>();


    public void Start() {
        //go through village states and create random hp for the houses
        for(int x = 0; x < VillageStates.Count; x++) {
            VillageStates[x].id = x;
            for(int i = 0; i < VillageStates[x].houses.Count; i++) {
                VillageStates[x].houses[i].maxHealth = Random.Range(2, 5);
                VillageStates[x].houses[i].health = VillageStates[x].houses[i].maxHealth;
                VillageStates[x].houses[i].id = i;
            }
        }
    }
    public void ResetSpyProgress() {
        SpyProgress = 0;
        //update spy meter
    }

    public void UpdateVillageState(int id, Village.VillageState newState) {
        VillageStates[id].villageState = newState;
        if(newState == Village.VillageState.DESTROYED)
            VillageStates[id].isHeroPresent = false;
        if(newState == Village.VillageState.DEFENDED) {
            //hero repelled orcs, only leave once spy is defeated or leaves too
        }
    }

    //public void AddVillageData(string JSONData) {
    //    villageJSON.Add(JSONData);
    //}

    //public string GetVillageJSON(int id) {
    //    string dataString = villageJSON[id];
    //    villageJSON.RemoveAt(id);
    //    return dataString;
    //}

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
