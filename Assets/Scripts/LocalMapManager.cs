using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalMapManager : MonoBehaviour
{
    public int id; //this needs to be unique for each local map and match the village on the overworld
    public LocalMapState mapState;
    public GameObject heroPrefab;
    public GameObject orcPrefab;
    public List<HouseController> houses = new List<HouseController>();
    public GameObject orcSpawnObject; //where orcs will spawn from
    public float spawnTimer;
    public float spawnTime = 10f; //how long inbetween orc spawns
    public int sceneIndex = 0;


    // Start is called before the first frame update
    void Start() {
        //spawn all relative objects
        //LoadState();
        
    }

    // Update is called once per frame
    void Update() {
        spawnTimer += Time.deltaTime;
        if(spawnTimer >= spawnTime) {
            Instantiate(orcPrefab, orcSpawnObject.transform.position, Quaternion.identity);
            //orc script will pick a random house in the level
            spawnTimer = 0f;
            spawnTime = Random.Range(10f, 30f); //pick a random time for next spawn
        }
    }

    private void OnDestroy() {
        //SaveState(); //should be called whenever leaving scene
    }

    // public void SaveState() {
    //    //add data to mapStat instance
    //    mapState.id = id;
    //    HouseController[] houses = FindObjectsOfType<HouseController>();
    //    foreach (HouseController house in houses) {
    //        mapState.houses.Add(house);
    //    }
    //    HeroController hero = FindObjectOfType<HeroController>();
    //    if ( hero != null) {
    //        mapState.isHeroPresent = true;
    //        mapState.heroPosition = hero.transform.position;
    //    }

    //    //get all orcs
    //    OrcController[] orcControllers = FindObjectsOfType<OrcController>();
    //    foreach (OrcController orcController in orcControllers) {
    //        mapState.orcControllers.Add(orcController);
    //    }

    //    string villageSave = JsonUtility.ToJson(mapState);
    //    OverworldManager.Instance.AddVillageData(villageSave);
    // }

    // public void LoadState() {
    //    mapState = JsonUtility.FromJson<LocalMapState>(OverworldManager.Instance.GetVillageJSON(id)); //might need to loop through to find the right one

    //    int houseCount = mapState.houses.Count;
    //    HouseController[] houses = FindObjectsOfType<HouseController>();
    //    for (int i = 0; i < houseCount; i++) {
    //        for (int j = 0; j < houseCount; i++) {
    //            if (houses[i].id == mapState.houses[j].id) {
    //                //they match
    //                //houses[i] = mapState.houses[j];
    //                houses[i].health = mapState.houses[j].health;
    //            }
    //        }
    //    }
    //    if (mapState.isHeroPresent) {
    //        //spawn hero
    //        Instantiate(heroPrefab, mapState.heroPosition, Quaternion.identity);
    //    }
    //    foreach(OrcController orc in mapState.orcControllers) {
    //        //spawn new orc with info
    //        GameObject o = Instantiate(orcPrefab);
    //        o.GetComponent<OrcController>().health = orc.health;
    //    }
    // }

    public void LoadState() {
       Debug.Log("loading state");
       //get appropriate village data
       LocalMapState state = OverworldManager.Instance.VillageStates[id];
       for (int i = 0; i < state.houses.Count; i++) {
           houses[i].health = state.houses[i].health;
           houses[i].maxHealth = state.houses[i].maxHealth;
           if (houses[i].health <= 0) {
               //set house to destroyed state without playing effects
               houses[i].gameObject.SetActive(false);
               Debug.Log("House was already destroyed");
           }
       }

       //spawn orcs
       for (int i = 0; i <= state.orcCount; i++) {
           Debug.Log("Spawning Orc");
           //spawn new orc with info
           GameObject o = Instantiate(orcPrefab);
           //give random location
           //o.transform.position = Random...
       }

       if (state.isHeroPresent) {
           Debug.Log("Spawning Hero");
           //spawn hero
           GameObject o = Instantiate(heroPrefab);
           //move hero to center
           o.transform.position = Vector2.zero;
       }
    }

    // public void CheckVillageStatus() {
    //    //called when a house is destroyed or orc defeated
    //    //get all orcs in level
    //    OrcController[] orcs = FindObjectsOfType<OrcController>();
    //    if (orcs.Length > 0) {
    //        //still orcs, so village is still being raided
    //        Debug.Log("Orcs still in Village");
    //    }
    //    else {
    //        //no orcs, change status?
    //        OverworldManager.Instance.UpdateVillageState(id, Village.VillageState.DEFENDED);
    //        Debug.Log("Village Defended");
    //    }
    //    //update orc count
    //    OverworldManager.Instance.VillageStates[id].orcCount = orcs.Length;
    //    //see if any houses are still standing
    //    foreach (HouseController house in houses) {
    //        if(house.health > 0) {
    //            //still standing, so stay in level
    //            Debug.Log("Houses still standing");
    //            return;
    //        }
    //    }
    //    PlayerController playerController = FindObjectOfType<PlayerController>();
    //    //no houses left
    //    //leave level
    //    Debug.Log("leaving Level");
    //    OverworldManager.Instance.UpdateVillageState(id, Village.VillageState.DESTROYED);
    //    OverworldManager.Instance.UpdateSpyMeter(playerController.spy_progress);
    //    SceneManager.LoadScene(0);
    // }

    public void PlayerDied() {
        //Called because the hero killed you, loser
        //OverworldManager.Instance.PlayerDied(id);
        //reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadLevel() {
        //load next level
        SceneManager.LoadScene(sceneIndex);
    }
}


[System.Serializable]
public class LocalMapState
{
    public int id; //id corresponds to the number at the end of the village name
    //public List<HouseController> houses = new List<HouseController>();
    public List<HouseState> houses = new List<HouseState>();
    public bool isHeroPresent = false;
    //public Vector2 heroPosition = Vector2.zero;
    //public List<OrcController> orcControllers = new List<OrcController>();
    public int orcCount = 0;
    public Village.VillageState villageState;
}

[System.Serializable]
public class HouseState
{
    public int id; //unique for each house
    public int health;
    public int maxHealth;
}
