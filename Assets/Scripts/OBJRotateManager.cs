using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBJRotateManager : MonoBehaviour
{
    private MeshFilter mCurrentMeshFilter;

    private void Start()
    {
        mCurrentMeshFilter = GetComponent<MeshFilter>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Mesh mesh = mCurrentMeshFilter.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            Vector3[] newVertices = new Vector3[vertices.Length];
            Quaternion rotation = transform.rotation;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertex = vertices[i];
                newVertices[i] = rotation * vertex;
            }
            mesh.vertices = newVertices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            transform.rotation = Quaternion.identity;
        }
    }
}
