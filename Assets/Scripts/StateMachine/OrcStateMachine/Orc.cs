using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class Orc : MonoBehaviour
{
    #region State Variables
    public OrcStateMachine StateMachine { get; private set; }
    public OrcAttackHouseState AttackHouseState { get; private set; }
    public OrcSeekHouseState SeekHouseState { get; private set; }
    public OrcStunnedState StunnedState { get; private set; }
    public OrcWaypointState WaypointState { get; private set; }
    public OrcIdleState IdleState { get; private set; }
    public OrcChaseState ChaseState { get; private set; }

    [SerializeField]
    private OrcData orcData;
    #endregion

    #region Components
    public MovementController2D MovementController { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public Animator Anim { get; private set; }

    public TMP_Text hpText;
    public AudioSource attackSound;
    public AudioSource hurtSound;

    #endregion

    #region Variables
    [HideInInspector]
    public GameObject targetGO;
    [HideInInspector]
    public int health;
    [HideInInspector]
    public GameObject[] houses;
    [HideInInspector]
    public Waypoint waypoint;
    [HideInInspector]
    public bool waypointActive;
    private int maxHealth;
    #endregion

    #region Unity Callbacks
    private void Awake() {
        StateMachine = new OrcStateMachine();
        AttackHouseState = new OrcAttackHouseState(this, StateMachine, orcData, "idle"); //chose idle so that I can transition into attack during the state
        SeekHouseState = new OrcSeekHouseState(this, StateMachine, orcData, "seek");
        StunnedState = new OrcStunnedState(this, StateMachine, orcData, "stunned");
        WaypointState = new OrcWaypointState(this, StateMachine, orcData, "waypoint");
        IdleState = new OrcIdleState(this, StateMachine, orcData, "idle");
        ChaseState = new OrcChaseState(this, StateMachine, orcData, "chase");

        //get components
        MovementController = GetComponent<MovementController2D>();
        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
    }

    private void Start() {
        StateMachine.Initialize(SeekHouseState);
        //pick a random amount of health
        health = Mathf.RoundToInt(Random.Range(orcData.minHealth, orcData.maxHealth));
        maxHealth = health;
        //update HP text
        UpdateHealthText();
    }

    private void Update() {
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate() {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.collider.CompareTag("Hero")) {
            TakeDamage();
            StateMachine.ChangeState(StunnedState);
            if (collision.contactCount > 0)
                RB.AddForce(collision.contacts[0].normal * 10f, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Hero")) {
            if (StateMachine.CurrentState != WaypointState && StateMachine.CurrentState is not OrcCombatState){
                targetGO = collision.gameObject;
                StateMachine.ChangeState(ChaseState);
            }
        }
    }

    #endregion

    #region Functions
    /// <summary>
    /// 
    /// </summary>
    /// <returns>Array of non destroyed houses, sorted by distance</returns>
    public GameObject[] FindValidHouses() {
        HouseController[] houses = FindObjectsByType<HouseController>(FindObjectsSortMode.None);
        List<GameObject> houseGOs = new List<GameObject>();
        //check if house is not destroyed
        foreach (HouseController house in houses) {
            if (!house.destroyed) {
                houseGOs.Add(house.gameObject);
            }
        }
        return houseGOs.OrderBy(d => (transform.position - d.transform.position).sqrMagnitude).ToArray(); //not exactly sure how this works. "d" should be like the "house" in the foreach
    }

    private void TakeDamage() {
        health -= 1;
        //play sound and anim
        hurtSound.Play();
        
        UpdateHealthText();
        if (health <= 0) {
            //destroy orc
            Destroy(gameObject);
            //update local status
            //FindObjectOfType<LocalMapManager>().CheckVillageStatus();
        }

    }

    public void UpdateHealthText() {
        hpText.text = "Oni\n" + health + "\\" + maxHealth;
    }

    public void AssignWaypoint(Waypoint newWaypoint) {
        if(CompareWaypoints(newWaypoint)) {
            //already assigned, already moving to it or has already reached it
            //do nothing
            Debug.Log("Waypoints match");
        }
        else {
            waypoint = newWaypoint;
            waypointActive = true;
            Debug.Log("assigned waypoint");
        }
    }

    public bool CompareWaypoints(Waypoint newWaypoint) {
        return ReferenceEquals(newWaypoint, waypoint);
    }

    #endregion
}
