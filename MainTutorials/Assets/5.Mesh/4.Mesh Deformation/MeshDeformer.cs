using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//***********************************写的比较急，还没有细看！！！！！！！！！！！！！***********************************

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour
{
    Mesh deformingMesh;
    Vector3[] originalVertices, displaycedVertices;
    Vector3[] vertexVelocities;

    float springForce = 20f;

    float damping = 5f;

    float uniformScale = 1f;


    void Start()
    {
        deformingMesh = GetComponent<MeshFilter>().mesh;
        originalVertices = deformingMesh.vertices;
        displaycedVertices = new Vector3[originalVertices.Length];

        for (int i = 0; i < originalVertices.Length; i++)
        {
            displaycedVertices[i] = originalVertices[i];
        }

        vertexVelocities = new Vector3[originalVertices.Length];
    }

    public void AddDeformingForce(Vector3 point, float force)
    {
        point = transform.InverseTransformPoint(point);
        for (int i = 0; i < displaycedVertices.Length; i++)
        {
            AddForceToVertex(i, point, force);
        }
    }

    void AddForceToVertex(int i, Vector3 point, float force)
    {
        Vector3 pointToVertex = displaycedVertices[i] - point;
		pointToVertex *= uniformScale;
        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
        float velocity = attenuatedForce * Time.deltaTime;
        vertexVelocities[i] += pointToVertex.normalized * velocity;
    }

    void Update()
    {
        uniformScale = transform.localScale.x;
        for (int i = 0; i < displaycedVertices.Length; i++)
        {
            UpdateVertex(i);
        }

        deformingMesh.vertices = displaycedVertices;
        deformingMesh.RecalculateNormals();
    }

    void UpdateVertex(int i)
    {
        Vector3 velocity = vertexVelocities[i];
        Vector3 displacement = displaycedVertices[i] - originalVertices[i];
		displacement *= uniformScale;
        velocity -= displacement * springForce * Time.deltaTime;
        velocity *= 1f - damping * Time.deltaTime;
        vertexVelocities[i] = velocity;
        displaycedVertices[i] += velocity * (Time.deltaTime / uniformScale);
    }
}
