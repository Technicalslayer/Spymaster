using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalMapManager : MonoBehaviour
{
    public int id; //this needs to be unique for each local map
    public LocalMapState mapState;


    // Start is called before the first frame update
    void Start()
    {
        //spawn all relative objects
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveState() {
        //add data to mapStat instance
        mapState.id = id;
        HouseController[] houses = FindObjectsOfType<HouseController>();
        foreach (HouseController house in houses) {
            mapState.houses.Add(house);
        }
        
        string villageSave = JsonUtility.ToJson(mapState);
        OverworldManager.Instance.AddVillageData(villageSave);
    }

    public void LoadState() {
        mapState = JsonUtility.FromJson<LocalMapState>(OverworldManager.Instance.GetVillageJSON(id)); //might need to loop through to find the right one
        
        int houseCount = mapState.houses.Count;
        HouseController[] houses = FindObjectsOfType<HouseController>();
        for (int i = 0; i < houseCount; i++) {
            if (houses[i].id == mapState.houses[i].id) {

            }
        }
    }
}

[System.Serializable]
public class LocalMapState
{
    public int id; //id corresponds to the number at the end of the village name
    public List<HouseController> houses = new List<HouseController>();
    public bool isHeroPresent = false;
    public Vector2 heroPosition = Vector2.zero;
    public List<OrcController> orcControllers = new List<OrcController>();
}
