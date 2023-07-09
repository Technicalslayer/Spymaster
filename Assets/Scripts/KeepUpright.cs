using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepUpright : MonoBehaviour
{
    public Transform parent;
    public Vector2 upDir = Vector2.up; //what direction to keep object facing
    public float offset = 0.5f; //how far from gameobject to stay

    private void Start() {
        parent = transform.parent;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = parent.position + new Vector3(0, offset, 0);
        transform.rotation = Quaternion.identity;
    }
}
