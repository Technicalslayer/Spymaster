using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object. Stores data for states/state machine to use.
/// </summary>
[CreateAssetMenu(fileName = "newHeroData", menuName = "Data/Hero Data/Base Data")]
public class HeroData : ScriptableObject
{
    [Tooltip("Layers of potential targets")]
    public LayerMask targetLayers;
    [Tooltip("Layers of vision blocking obstacles")]
    public LayerMask obstacleLayer;

    [Header("Patrol State")]
    [Tooltip("Walk speed")]
    public float patrolSpeed = 5f;
    public float patrolTurnSpeed = 180f;
    [Tooltip("How long for player to be in view before chasing")]
    public float detectionTime = 1.5f;
    [Tooltip("Negative Value\nHow fast the detection meter drops back to 0")]
    public float detectionDecreaseRate = -0.5f;

    [Header("Look Around State")]
    [Tooltip("Minimum time to spend looking in a single direction")]
    public float minLookTime = 0f;
    [Tooltip("Maximum time to spend looking in a single direction")]
    public float maxLookTime = 2f;
    [Tooltip("Degrees per second")]
    public float minTurnSpeed = 50f;
    [Tooltip("Degrees per second")]
    public float maxTurnSpeed = 120f;

    [Header("Suspicious State")]
    public float suspiciousMoveSpeed = 2f;
    public float suspiciousTurnSpeed = 180f;
    [Tooltip("How long to search before giving up")]
    public float suspiciousTime = 5f;
    [Tooltip("Max time to look in a single direction")]
    public float suspiciousLookTime = 1f;

    [Header("Chase State")]
    public float chaseSpeed = 6.5f;
    public float chaseTurnSpeed = 180f;
    [Tooltip("How long the target can be out of sight before changing states")]
    public float chaseSearchTime = 3f;
    [Tooltip("How long between calls to update movement controller")]
    public float movementUpdateTime = 0.1f;

    [Header("Search State")]
    [Tooltip("How long to search before giving up")]
    public float searchTime = 10f;

    [Header("Stunned State")]
    public float stunTime = 0.7f;

    [Header("Repair State")]
    [Tooltip("How long between repairing 1 point of damage")]
    public float repairTime = 0.5f;

    [Header("Confuse State")]
    [Tooltip("How long to be confused for")]
    public float confuseTime = 10f;
}
