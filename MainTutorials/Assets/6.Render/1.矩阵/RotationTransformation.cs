﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocalTransform
{
    public class RotationTransformation : Transformation
    {
        public Vector3 rotation;

        public override Matrix4x4 Matrix
        {
            get
            {
                float radX = rotation.x * Mathf.Deg2Rad;
                float radY = rotation.y * Mathf.Deg2Rad;
                float radZ = rotation.z * Mathf.Deg2Rad;
                float sinX = Mathf.Sin(radX);
                float cosX = Mathf.Cos(radX);
                float sinY = Mathf.Sin(radY);
                float cosY = Mathf.Cos(radY);
                float sinZ = Mathf.Sin(radZ);
                float cosZ = Mathf.Cos(radZ);


                Matrix4x4 matrix = new Matrix4x4();
                //分别绕X，Y，Z三个轴旋转的矩阵相乘得到的新矩阵 Y x Z, X x(Y x Z)
                //x轴
                matrix.SetColumn(0, new Vector4(
                        cosY * cosZ,
                        cosX * sinZ + sinX * sinY * cosZ,
                        sinX * sinZ - cosX * sinY * cosZ,
                        0f
                    ));

                matrix.SetColumn(1, new Vector4(
                        -cosY * sinZ,
                        cosX * cosZ - sinX * sinY * sinZ,
                        sinX * cosZ + cosX * sinY * sinZ,
                        0f
                    ));

                matrix.SetColumn(2, new Vector4(
                        sinY,
                        -sinX * cosY,
                        cosX * cosY,
                        0f
                    ));

                matrix.SetColumn(3, new Vector4(0f, 0f, 0f, 1f));
                return matrix;
            }
        }
    }

}
