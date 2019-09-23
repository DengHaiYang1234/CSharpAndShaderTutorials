using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Tests
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Grid : MonoBehaviour
    {
        public int xSize, ySize;

        private Vector3[] vertices;

        private Mesh mesh;

        private void Awake()
        {
            Generate();
        }

        void Generate()
        {
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.name = "Procedural Grid";
            vertices = new Vector3[(xSize + 1) * (ySize + 1)];
            Vector4[] tangents = new Vector4[vertices.Length];
            Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
            Vector2[] uv = new Vector2[vertices.Length];
            for (int i = 0, y = 0; y <= ySize; y++)
            {
                for (int x = 0; x <= xSize; x++, i++)
                {
                    vertices[i] = new Vector3(x, y);
                    uv[i] = new Vector2((float)x/xSize, (float)y/ySize);
                    tangents[i] = tangent;
                }
            }
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.tangents = tangents;
            //顺时针
            int[] triangles = new int[xSize * ySize * 6];//总共的三角形个数
                                                         //ti:6个点构成两个三角形。最终形成一个平行四边形
            for (int ti = 0, vi = 0, y = 0; y < ySize ; y++, vi++)
            {
                for (int x = 0; x < xSize; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;//左下点
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;//右下点
                    triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;//左上点
                    triangles[ti + 5] = vi + xSize + 2;//右上点
                }
            }
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

        }

        private void OnDrawGizmos()
        {
            if (vertices == null)
                return;
            Gizmos.color = Color.black;
            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.DrawSphere(vertices[i], 0.1f);
            }
        }

    }
}

