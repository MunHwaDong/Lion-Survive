using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurdle : MonoBehaviour
{
    [SerializeField] private GameObject checkPoint;
    
    void Start()
    {
        var mesh = GetComponent<MeshFilter>().mesh;
        
        Debug.Log(mesh.bounds);
        
        foreach (Vector3 meshVertex in mesh.vertices)
        {
            Instantiate(checkPoint, transform.TransformPoint(meshVertex), Quaternion.identity);
            Debug.Log(meshVertex);
        }
    }
}
