using UnityEngine;
using System.Collections;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RoundedCube : MonoBehaviour
{

    public int xSize, ySize, zSize, roundness;
    private Mesh mesh;
    private Vector3[] vertices;
    //法线
    Vector3[] normals;
    Color32[] cubeUV;

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
        CreateColliders();
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
        normals = new Vector3[vertices.Length];
        cubeUV = new Color32[vertices.Length];

        int v = 0;
        //y：高度
        for (int y = 0; y <= ySize; y++)
        {
            //x边
            for (int x = 0; x <= xSize; x++)
            {
                SetVertex(v++, x, y, 0);
            }
            //z边
            for (int z = 1; z <= zSize; z++)
            {
                SetVertex(v++, xSize, y, z);
            }
            //与上面x边平行的另一边
            for (int x = xSize - 1; x >= 0; x--)
            {
                SetVertex(v++, x, y, zSize);
            }
            //与上面z边平行的另一边
            for (int z = zSize - 1; z > 0; z--)
            {
                SetVertex(v++, 0, y, z);
            }
        }

        //填充每个ySize的xz平面顶点数
        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
            {
                SetVertex(v++, x, ySize, z);
            }
        }

        //填充每个底部的xz平面顶点数
        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
            {
                SetVertex(v++, x, 0, z);
            }
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.colors32 = cubeUV;
    }
    //四个面的三角    
    private void CreateTriangles()
    {
        //四边形数量之和
        int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
        //一个四边形是由6个顶点构成（不在乎是否有重合的顶点）
        int[] triangles = new int[quads * 6];

        int[] trianglesZ = new int[(xSize * ySize) * 12];//12 : 2 * 6  两面*三角形数量
        int[] trianglesX = new int[(ySize * zSize) * 12];
        int[] trianglesY = new int[(xSize * zSize) * 12];

        //完整的在y层四条边的顶点数
        int ring = (xSize + zSize) * 2;
        //t：起始顶点索引（6个一轮回）  v：起始递增索引
        int tZ = 0, tX = 0, tY = 0, t = 0, v = 0;

        for (int y = 0; y < ySize; y++, v++)
        {
            for (int q = 0; q < xSize; q++, v++)
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }

            for (int q = 0; q < zSize; q++, v++)
            {
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < xSize; q++, v++)
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < zSize - 1; q++, v++)
            {
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
            }
            //最后一个点的右下和右上需要回到初始点
            tX = SetQuad(trianglesX, tX, v, v - ring + 1, v + ring, v + 1);
        }

        tY = CreatTopFace(trianglesY, tY, ring);
        tY = CreatBottomFace(trianglesY, tY, ring);

        mesh.subMeshCount = 3;
        mesh.SetTriangles(trianglesZ,0);
        mesh.SetTriangles(trianglesX,1);
        mesh.SetTriangles(trianglesY,2);
        //mesh.triangles = triangles;
    }


    /// <summary>
    /// 构建顶面
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
            //第一排1，2
            t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
        }

        //第一排最后一个顶点特殊处理
        t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

        //z方向第三个顶点
        int vMin = ring * (ySize + 1) - 1;
        //z方向第三个顶点的右顶点
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

        int vTop = vMin - 2;
        //第四排第一个
        t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
        for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
        {
            //第四排第二个
            t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
        }

        //第四排最后一个
        t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);
        return t;
    }

    /// <summary>
    /// 构建底面
    /// </summary>
    /// <param name="triangles"> 已构成的三角顶点 </param>
    /// <param name="t"> 初始顶点索引 </param>
    /// <param name="ring"> 边的索引 </param>
    /// <returns></returns>
    int CreatBottomFace(int[] triangles, int t, int ring)
    {
        int v = 1;
        int vMid = vertices.Length - (xSize - 1) * (zSize - 1);
        t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
        for (int x = 1; x < xSize - 1; x++, v++, vMid++)
        {
            t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
        }
        t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

        int vMin = ring - 2;
        vMid -= xSize - 2;
        int vMax = v + 2;

        for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(triangles, t, vMin, vMid + xSize - 1, vMin + 1, vMid);
            for (int x = 1; x < xSize - 1; x++, vMid++)
            {
                t = SetQuad(
                    triangles, t,
                    vMid + xSize - 1, vMid + xSize, vMid, vMid + 1);
            }
            t = SetQuad(triangles, t, vMid + xSize - 1, vMax + 1, vMid, vMax);
        }

        int vTop = vMin - 1;
        t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
        for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
        }
        t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);
        return t;
    }

    void CreateColliders()
    {
        AddBoxCollider(xSize,ySize - roundness * 2,zSize - roundness * 2);
        AddBoxCollider(xSize - roundness * 2,ySize,zSize - roundness * 2);
        AddBoxCollider(xSize - roundness * 2,ySize - roundness * 2,zSize);

        Vector3 min = Vector3.one * roundness;
		Vector3 half = new Vector3(xSize, ySize, zSize) * 0.5f; 
		Vector3 max = new Vector3(xSize, ySize, zSize) - min;

		AddCapsuleCollider(0, half.x, min.y, min.z);
		AddCapsuleCollider(0, half.x, min.y, max.z);
		AddCapsuleCollider(0, half.x, max.y, min.z);
		AddCapsuleCollider(0, half.x, max.y, max.z);
		
		AddCapsuleCollider(1, min.x, half.y, min.z);
		AddCapsuleCollider(1, min.x, half.y, max.z);
		AddCapsuleCollider(1, max.x, half.y, min.z);
		AddCapsuleCollider(1, max.x, half.y, max.z);
		
		AddCapsuleCollider(2, min.x, min.y, half.z);
		AddCapsuleCollider(2, min.x, max.y, half.z);
		AddCapsuleCollider(2, max.x, min.y, half.z);
		AddCapsuleCollider(2, max.x, max.y, half.z);
    }

    void AddBoxCollider(float x,float y,float z)
    {
        BoxCollider c = gameObject.AddComponent<BoxCollider>();
        c.size = new Vector3(x,y,z);
    }

    private void AddCapsuleCollider (int direction, float x, float y, float z) {
		CapsuleCollider c = gameObject.AddComponent<CapsuleCollider>();
		c.center = new Vector3(x, y, z);
		c.direction = direction;
		c.radius = roundness;
		c.height = c.center[direction] * 2f;
	}

    void SetVertex(int i, int x, int y, int z)
    {
        Vector3 inner = vertices[i] = new Vector3(x, y, z);


        //法线计算
        if (x < roundness)
        {
            inner.x = roundness;
        }
        else if (x > xSize - roundness)
        {
            inner.x = xSize - roundness;
        }

        if (y < roundness)
        {
            inner.y = roundness;
        }
        else if (y > ySize - roundness)
        {
            inner.y = ySize - roundness;
        }

        if (z < roundness)
        {
            inner.z = roundness;
        }
        else if (z > zSize - roundness)
        {
            inner.z = zSize - roundness;
        }

        normals[i] = (vertices[i] - inner).normalized;
        vertices[i] = inner + normals[i] * roundness;
        cubeUV[i] = new Color32((byte)x,(byte)y,(byte)z,0);
    }



    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(vertices[i], 0.1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(vertices[i], normals[i]);
        }
    }
}
