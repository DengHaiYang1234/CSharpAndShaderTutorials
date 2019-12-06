using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocalTransform
{
    public class ScaleTransformation : Transformation
    {
        public Vector3 Scale;
        public Vector3 Apply(Vector3 point)
        {
            point.x *= Scale.x;
            point.y *= Scale.y;
            point.z *= Scale.z;
            return point;
        }

        public override Matrix4x4 Matrix
        {
            get
            {
                Matrix4x4 matrix = new Matrix4x4();
                matrix.SetRow(0, new Vector4(Scale.x, 0f, 0f, 0f));
                matrix.SetRow(1, new Vector4(0f, Scale.y, 0f, 0f));
                matrix.SetRow(2, new Vector4(0f, 0f, Scale.z, 0f));
                matrix.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
                return matrix;
            }
        }

    }
}

