using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FieldOfView : MonoBehaviour
{
    public float viewDistance;
    [Range(0, 360)]
    public float viewAngle; 
    public float viewResolution; //how many lines to cast out for each degree
    public LayerMask obstacleMask;


    private MeshFilter viewMeshFilter;
    private Mesh viewMesh;

    // Start is called before the first frame update
    void Start()
    {
        //get components
        viewMeshFilter = GetComponent<MeshFilter>();
        viewMesh = new Mesh();
        viewMesh.name = "View mesh";
        viewMeshFilter.mesh = viewMesh;


    }

    // Update is called once per frame
    void LateUpdate()
    {
        DrawFieldOfView();
    }

    private void DrawFieldOfView(){
        int stepCount = Mathf.RoundToInt(viewAngle * viewResolution); //total number of steps to draw
        float stepAngleSize = viewAngle / stepCount; //size of each step in degrees
        List<Vector3> viewPoints = new List<Vector3>();

        for(int i = 0; i <= stepCount; i++){
            float angle = -transform.eulerAngles.z - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            viewPoints.Add(newViewCast.point);
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];
        vertices[0] = Vector3.zero;;

        for(int i = 0; i < vertexCount - 1; i++){
            vertices[i+1] = transform.InverseTransformPoint(viewPoints[i]);

            if(i < vertexCount - 2){
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    private ViewCastInfo ViewCast(float globalAngle){
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewDistance, obstacleMask);
         
        if(hit){
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else{
            return new ViewCastInfo(false, transform.position + dir * viewDistance, viewDistance, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal){
        if(!angleIsGlobal){
            angleInDegrees += transform.eulerAngles.z;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }

    public struct ViewCastInfo {
        public bool hit;
        public Vector2 point;
        public float dist;
        public float angle;

        public ViewCastInfo(bool _hit, Vector2 _point, float _dist, float _angle){
            hit = _hit;
            point = _point;
            dist = _dist;
            angle = _angle;
        }
    }
}
