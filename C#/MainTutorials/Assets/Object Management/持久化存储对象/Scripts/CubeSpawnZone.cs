using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawnZone : SpawnZone
{
    [SerializeField]
    private bool surfaceOnly;


    public override Vector3 SpawnPoint
    {
        get
        {
            Vector3 p;
            p.x = UnityEngine.Random.Range(-0.5f, 0.5f);
            p.y = UnityEngine.Random.Range(-0.5f, 0.5f);
            p.z = UnityEngine.Random.Range(-0.5f, 0.5f);
            if (surfaceOnly)
            {
                int axis = (int)UnityEngine.Random.Range(0f, 3f);
                p[axis] = p[axis] < 0f ? -0.5f : 0.5f;
            }

            return transform.TransformPoint(p);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }
}
