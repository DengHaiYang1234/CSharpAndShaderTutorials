using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Cube : MonoBehaviour
{

    public int xSize, ySize, zSize;

    private Mesh mesh;
    private Vector3[] vertices;

    /// <summary>
    /// 构建三角形
    /// </summary>
    /// <param name="triangles"> 三角顶点 </param>
    /// <param name="i"> 初始顶点 </param>
    /// <param name="v00"> 左下 </param>
    /// <param name="v10"> 右下 </param>
    /// <param name="v01"> 左上 </param>
    /// <param name="v11"> 右上 </param>
    /// <returns></returns>
    private static int
    SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
    {
        triangles[i] = v00;
        triangles[i + 1] = triangles[i + 4] = v01;
        triangles[i + 2] = triangles[i + 3] = v10;
        triangles[i + 5] = v11;
        return i + 6;
    }

    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Cube";

        CreateVertices();
        CreateTriangles();
    }

    private void CreateVertices()
    {

        //八个角顶点
        int cornerVertices = 8;
        //12条边的顶点（每条边与另一条边会重合一个顶点）
        int edgeVertices = (xSize + ySize + zSize - 3) * 4;
        //6个面的顶点（构成面的边会有两个顶点存在重合。所以顶点数量-2）
        int faceVertices = (
            (xSize - 1) * (ySize - 1) +
            (xSize - 1) * (zSize - 1) +
            (ySize - 1) * (zSize - 1)) * 2;
        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];

        int v = 0;
        //y：高度
        for (int y = 0; y <= ySize; y++)
        {
            //x边
            for (int x = 0; x <= xSize; x++)
            {
                vertices[v++] = new Vector3(x, y, 0);
            }
            //z边
            for (int z = 1; z <= zSize; z++)
            {
                vertices[v++] = new Vector3(xSize, y, z);
            }
            //与上面x边平行的另一边
            for (int x = xSize - 1; x >= 0; x--)
            {
                vertices[v++] = new Vector3(x, y, zSize);
            }
            //与上面z边平行的另一边
            for (int z = zSize - 1; z > 0; z--)
            {
                vertices[v++] = new Vector3(0, y, z);
            }
        }

        //填充每个ySize的xz平面顶点数
        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
            {
                vertices[v++] = new Vector3(x, ySize, z);
            }
        }

        //填充每个底部的xz平面顶点数
        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
            {
                vertices[v++] = new Vector3(x, 0, z);
            }
        }

        mesh.vertices = vertices;
    }
    //四个面的三角    
    private void CreateTriangles()
    {
        //四边形数量之和
        int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
        //一个四边形是由6个顶点构成（不在乎是否有重合的顶点）
        int[] triangles = new int[quads * 6];
        //完整的在y层四条边的顶点数
        int ring = (xSize + zSize) * 2;
        //t：起始顶点索引（6个一轮回）  v：起始递增索引
        int t = 0, v = 0;

        for (int y = 0; y < ySize; y++, v++)
        {
            for (int q = 0; q < ring - 1; q++, v++)
            {
                t = SetQuad(triangles, t, v, v + 1, v + ring, v + ring + 1);
            }

            //最后一个点的右下和右上需要回到初始点
            t = SetQuad(triangles, t, v, v - ring + 1, v + ring, v + 1);
        }

        t = CreatTopFace(triangles, t, ring);

        mesh.triangles = triangles;
    }


    /// <summary>
    /// 构建顶面或底面
    /// </summary>
    /// <param name="triangles"> 已构成的三角顶点 </param>
    /// <param name="t"> 初始顶点索引 </param>
    /// <param name="ring"> 边的索引 </param>
    /// <returns></returns>
    int CreatTopFace(int[] triangles, int t, int ring)
    {
        //最顶端的x边的第一个顶点
        int v = ring * ySize;
        for (int x = 0; x < xSize - 1; x++, v++)
        {
            t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
        }

        //最后一个顶点特殊处理
        t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

        int vMin = ring * (ySize + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;
        for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
        {
            //第二/三排第一个
            t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + xSize - 1);
            //第二/三排第二个
            for (int x = 1; x < xSize - 1; x++, vMid++)
            {
                t = SetQuad(triangles, t, vMid, vMid + 1, vMid + xSize - 1, vMid + xSize);
            }
            //第二/三排第三个
            t = SetQuad(triangles, t, vMid, vMax, vMid + xSize - 1, vMax + 1);
        }
        return t;
    }



    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}