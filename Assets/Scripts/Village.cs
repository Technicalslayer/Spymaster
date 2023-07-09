using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Village : MonoBehaviour
{
    public enum VillageState { NORMAL, UNDER_ATTACK, DEFENDED, DESTROYED, CLASHING }
    
    public Sprite normalSprite;
    public Sprite underAttackSprite;
    public Sprite beingDefendedSprite;
    public Sprite destroyedSprite;
    public Sprite clashingSprite;
    public VillageState villageStatus;

    public bool heroPresent = false;
    public int orcCount = 0;
    public int sceneIndex = 0; //scene index to load
    public int id; //unique for each village
    

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateState(OverworldManager.Instance.VillageStates[id].villageState);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Updates if village is under attack or destroyed
    /// </summary>
    public void UpdateState(VillageState status) {
        OverworldManager.Instance.VillageStates[id].villageState = status;
        //switch statement based on keyword passed
        switch (status) {
            case VillageState.NORMAL: {
                    spriteRenderer.sprite = normalSprite;
                    villageStatus = VillageState.NORMAL;
                    break;
                }
            case VillageState.UNDER_ATTACK: {
                    spriteRenderer.sprite = underAttackSprite;
                    villageStatus = VillageState.UNDER_ATTACK;
                    break;
                }
            case VillageState.DEFENDED: {
                    spriteRenderer.sprite = beingDefendedSprite;
                    villageStatus = VillageState.DEFENDED;
                    break;
                }
            case VillageState.CLASHING: {
                    spriteRenderer.sprite = clashingSprite;
                    villageStatus = VillageState.CLASHING;
                    break;
                }
            case VillageState.DESTROYED: {
                    spriteRenderer.sprite = destroyedSprite;
                    villageStatus = VillageState.DESTROYED;
                    break;
                }
            default: {
                    Debug.LogError("No status passed");
                    return;
                }

        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        //check what collided and move into village if it makes sense
        if (collision.collider.tag == "Hero") {
            //check for orcs
            if (villageStatus == VillageState.UNDER_ATTACK) {
                //enter village
                //Destroy(collision.gameObject);
                heroPresent = true;
                collision.gameObject.SetActive(false);
                UpdateState(VillageState.CLASHING);
                OverworldManager.Instance.VillageStates[id].isHeroPresent = true;
            }
        }
        else if(collision.collider.tag == "Orc") {
            if (villageStatus != VillageState.DESTROYED) {
                //enter village
                orcCount += collision.gameObject.GetComponent<OverworldNPCController>().orcCount;
                Destroy(collision.gameObject);
                if (!heroPresent)
                    UpdateState(VillageState.UNDER_ATTACK);
                else
                    UpdateState(VillageState.CLASHING);
            }
            OverworldManager.Instance.VillageStates[id].orcCount = orcCount;
        }
        else if(collision.collider.tag == "Player") {
            //enter village change scene
            Debug.Log("changing levels");
            SceneManager.LoadScene(sceneIndex);
        }
    }
}
