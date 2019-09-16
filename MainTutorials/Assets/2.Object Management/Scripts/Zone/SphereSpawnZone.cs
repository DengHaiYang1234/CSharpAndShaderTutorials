using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSpawnZone : SpawnZone
{
    [SerializeField] private bool surfaceOnly;

    public override Vector3 SpawnPoint
    {
        get { return transform.TransformPoint(surfaceOnly ? Random.onUnitSphere : Random.insideUnitSphere); }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(Vector3.zero, 1f);
    }
}
