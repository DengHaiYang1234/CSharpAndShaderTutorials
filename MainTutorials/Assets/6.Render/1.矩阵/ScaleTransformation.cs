using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTransformation : Transformation
{
    public Vector3 Scale;
    public override Vector3 Apply(Vector3 point)
    {
        point.x *= Scale.x;
        point.y *= Scale.y;
        point.z *= Scale.z;
        return point;
    }

}
