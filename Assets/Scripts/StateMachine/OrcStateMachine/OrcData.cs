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
    public int Health;
    public int MaxHealth;
    public int MinHealth;

    [Header("StunnedState")]
    public float stunTime;
}
