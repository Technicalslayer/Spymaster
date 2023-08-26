using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PatrolHelper))]
public class PatrolEditor : Editor
{
    

    private void OnSceneGUI() {
        //get selected gameobject
        PatrolHelper t = target as PatrolHelper;

        if (t != null) {
            //print position
            t.UpdatePositionUI();
        }

        
    }
}
