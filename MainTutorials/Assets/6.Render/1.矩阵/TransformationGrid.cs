﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocalTransform
{
    public class TransformationGrid : MonoBehaviour
    {
        public Transform prefab;

        public int gridResolution = 10;

        Transform[] grid;

        List<Transformation> transformtions;

        private Matrix4x4 transformation;

        void Awake()
        {
            grid = new Transform[gridResolution * gridResolution * gridResolution];
            for (int i = 0, z = 0; z < gridResolution; z++)
            {
                for (int y = 0; y < gridResolution; y++)
                {
                    for (int x = 0; x < gridResolution; x++, i++)
                    {
                        grid[i] = CreatGridPoint(x, y, z);
                    }
                }
            }

            transformtions = new List<Transformation>();

        }

        Transform CreatGridPoint(int x, int y, int z)
        {
            Transform point = Instantiate<Transform>(prefab);
            point.localPosition = GetCoordinates(x, y, z);
            point.GetComponent<MeshRenderer>().material.color = new Color(
                (float)x / gridResolution,
                (float)y / gridResolution,
                (float)z / gridResolution
            );
            return point;
        }

        //从左至右排列坐标
        Vector3 GetCoordinates(int x, int y, int z)
        {
            return new Vector3(
                x - (gridResolution - 1) * 0.5f,
                y - (gridResolution - 1) * 0.5f,
                z - (gridResolution - 1) * 0.5f
            );
        }

        void Update()
        {
            UpdateTransformation();
            //GetComponents<Transformation>(transformtions);
            for (int i = 0, z = 0; z < gridResolution; z++)
            {
                for (int y = 0; y < gridResolution; y++)
                {
                    for (int x = 0; x < gridResolution; x++, i++)
                    {
                        grid[i].localPosition = TransformPoint(x, y, z);
                    }
                }
            }
        }

        Vector3 TransformPoint(int x, int y, int z)
        {
            Vector3 coordinates = GetCoordinates(x, y, z);
            return transformation.MultiplyPoint(coordinates);
        }

        void UpdateTransformation()
        {
            GetComponents<Transformation>(transformtions);
            if (transformtions.Count > 0)
            {
                //缩放矩阵
                transformation = transformtions[0].Matrix;
                for (int i = 1; i < transformtions.Count; i++)
                {
                    //缩放*位移*旋转矩阵,最终得到新的矩阵
                    transformation = transformtions[i].Matrix * transformation;
                }
            }
        }

    }
}

