using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object. Stores data for states/state machine to use.
/// </summary>
[CreateAssetMenu(fileName = "newHeroData", menuName = "Data/Hero Data/Base Data")]
public class HeroData : ScriptableObject
{
    [Header("Patrol State")]
    [Tooltip("Walk speed")]
    public float patrolSpeed = 5f;
    public float patrolTurnSpeed = 180f;

    [Header("Look Around State")]
    [Tooltip("Minimum time to spend looking in a single direction")]
    public float minLookTime = 0f;
    [Tooltip("Maximum time to spend looking in a single direction")]
    public float maxLookTime = 2f;
    [Tooltip("Degrees per second")]
    public float minTurnSpeed = 50f;
    [Tooltip("Degrees per second")]
    public float maxTurnSpeed = 120f;

    [Header("Chase State")]
    public float chaseSpeed = 6.5f;
    public float chaseTurnSpeed = 180f;
}
