using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : PersistableObject
{
    [SerializeField]
    Vector3 angularVelocity;

    private void FixedUpdate()
    {
        transform.Rotate(angularVelocity * Time.deltaTime);
    }

}
