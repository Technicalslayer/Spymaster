using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Scriptable object. Stores data for states/state machine to use.
/// </summary>
[CreateAssetMenu(fileName = "newOrcData", menuName = "Data/Orc Data/Base Data")]
public class OrcData : ScriptableObject
{
    [Header("Base Stats")]
    public int maxHealth;
    public int minHealth;

    [Header("Stunned State")]
    public float stunTime;

    [Header("Seek State")]
    public float seekSpeed;
    [Tooltip("How far to look for the closest house and target it instead of a random one.\nUseful to guide orcs to a specific house with a Waypoint.")]
    public float nonRandomSeekDistance;

    [Header("Chase State")]
    public float chaseSpeed;

    [Header("Idle State")]
    public float idleTime;
    public float idleSpeed;

    [Header("Pillage State")]
    [Tooltip("Speed used when seeking or attacking houses")]
    public float pillageSpeed;
    public float attackTime; //how long between attacks
}
