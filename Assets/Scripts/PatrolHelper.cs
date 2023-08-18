using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Will expand on this. Right now just used to print position to console.
/// </summary>
public class PatrolHelper : MonoBehaviour
{

    public void UpdatePositionUI() {
        Debug.Log("X: " + transform.position.x + ", Y: " + transform.position.y);
    }
}
