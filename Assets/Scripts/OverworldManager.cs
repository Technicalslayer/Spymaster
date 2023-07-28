using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public int SpyFailures = 0;
    public int SpyFailuresMax = 5; //how many attempts for a silver or better the player gets before losing
    public int SpySuccesses = 0;
    public int SpySuccessesMax = 5; //how many perfects the player needs to win

    //public int Resources = 0; //tracks amount of resources player has collected
    public List<GameObject> overworldObjects = new List<GameObject>(); //list of objects to enable/disable when entering/leaving overworld map
    public List<string> villageJSON = new List<string>(); //list of village data
    public List<LocalMapState> VillageStates = new List<LocalMapState>();
    public Image spyMeterImage;


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

    public void UpdateSpyMeter(int spyProgressAmount) {
        SpyProgress = spyProgressAmount;
        spyMeterImage.fillAmount = (float)spyProgressAmount / 100;
    }

    public void UpdateVillageState(int id, Village.VillageState newState) {
        VillageStates[id].villageState = newState;
        if(newState == Village.VillageState.DESTROYED)
            VillageStates[id].isHeroPresent = false;
        if(newState == Village.VillageState.DEFENDED) {
            //hero repelled orcs, only leave once spy is defeated or leaves too
        }
    }

    public void PlayerDied(int id) {
        //kick player and hero out of level
        LoadLevel(0);
        VillageStates[id].isHeroPresent = false;
        //should only happen if no orcs and village isn't destroyed
        //can later check if there are orcs present and then do something there
        //if (VillageStates[id].orcCount > 0) { }
        VillageStates[id].villageState = Village.VillageState.NORMAL;
    }

    public void LoadLevel(int sceneIndex) {
        SceneManager.LoadScene(sceneIndex);
    }

    public void AddVillageData(string JSONData) {
       villageJSON.Add(JSONData);
    }

    public string GetVillageJSON(int id) {
       string dataString = villageJSON[id];
       villageJSON.RemoveAt(id);
       return dataString;
    }

    // public void AddResources(int amount) {
    //    Resources += amount;
    //    if
    //    UpdateResourceCounter();
    // }

    /// <summary>
    /// Updates display
    /// </summary>
    public void UpdateResourceCounter() {
       return;
    }
}
