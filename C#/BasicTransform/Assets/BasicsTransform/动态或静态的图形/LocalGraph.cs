using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//外网：https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/
public class LocalGraph : MonoBehaviour
{
    public Transform pointPrefab;
    private int resolution = 50;
    Transform[] points;


    public bool isStatic = false;

    GraphFunction[] staticFuncs =
    {
        Square,
        Circle,
        Cylinder,
        FivePointedStar,
        CylinderWave,
        PeachShaped,
        Sphere,
        SphereInSphere,
        Doughnut,
    };

    GraphVector3Function[] vector3Functions =
    {
        SineVector3Function,
        Sine2DVector3Function,
        MultiSineVector3Function,
        MultiSine2DVector3Function,
        Vector3Ripple,
        Vector3Cylinder,
        Vector3Sphere,
        Vector3Torus
    };
    [Tooltip("动态")]
    public GraphVector3FunctionName vector3Function;
    [Tooltip("静态")]
    public GraphFunctionName funcs;

    const float pi = Mathf.PI;

    void Awake()
    {
        points = new Transform[resolution * resolution];
        float step = 2f / resolution;
        Vector3 scale = Vector3.one * step;
        for (int i = 0; i < points.Length; i++)
        {
            Transform point = Instantiate(pointPrefab);
            point.localScale = scale;
            point.SetParent(transform, false);
            points[i] = point;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float t = Time.time;

        if (isStatic)
        {
            GraphFunction fn;
            fn = staticFuncs[(int)funcs];

            float step = 2f / resolution;
            for (int i = 0, z = 0; z < resolution; z++)
            {
                //y
                float v = (z + 0.5f) * step - 1f;
                for (int x = 0; x < resolution; x++, i++)
                {
                    //x，z  X的取值被约束在【-1，1】，（0，0）为中点
                    float u = (x + 0.5f) * step - 1f;
                    points[i].localPosition = fn(u, v, t);
                }
            }
        }
        else
        {
            GraphVector3Function fn;
            fn = vector3Functions[(int)vector3Function];

            float step = 2f / resolution;
            for (int i = 0, z = 0; z < resolution; z++)
            {
                //y
                float v = (z + 0.5f) * step - 1f;
                for (int x = 0; x < resolution; x++, i++)
                {
                    //x，z
                    float u = (x + 0.5f) * step - 1f;
                    points[i].localPosition = fn(u, v, t);
                }
            }
        }
    }

    static Vector3 SineVector3Function(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;
        p.y = Mathf.Sin(pi * (x + t));
        p.z = z;
        return p;
    }
    static Vector3 MultiSineVector3Function(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;
        p.y = Mathf.Sin(pi * (x + t));
        p.y += Mathf.Sin(2f * pi * (x + 2f * t)) / 2f;// range: -1.5 to 1.5
        p.y *= 2f / 3f;
        p.z = z;
        return p;
    }
    static Vector3 Sine2DVector3Function(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;
        p.y = Mathf.Sin(pi * (x + t));
        p.y += Mathf.Sin(pi * (z + t));
        p.y *= 0.5f;
        p.z = z;
        return p;
    }
    static Vector3 MultiSine2DVector3Function(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;
        p.y = 4f * Mathf.Sin(pi * (x + z + t * 0.5f));
        p.y += Mathf.Sin(pi * (x + t));
        p.y += Mathf.Sin(2f * pi * (z + 2f * t)) * 0.5f;
        p.y *= 1 / 5.5f;
        p.z = z;
        return p;
    }
    static Vector3 Vector3Ripple(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;

        float d = Mathf.Sqrt(x * x + z * z);

        p.y = Mathf.Sin(pi * (4f * d - t));
        p.y /= 1f + 10f * d;
        p.z = z;
        return p;
    }
    static Vector3 Vector3Cylinder(float u, float v, float t)
    {
        Vector3 p;
        float r = 0.8f + Mathf.Sin(pi * (6f * u + 2f * v + t)) * 0.2f;
        p.x = r * Mathf.Sin(pi * u);
        p.y = v;
        p.z = r * Mathf.Cos(pi * u);
        return p;
    }
    static Vector3 Vector3Sphere(float u, float v, float t)
    {
        Vector3 p;
        float r = 0.8f + Mathf.Sin(pi * (6f * u + t)) * 0.1f;
        r += Mathf.Sin(pi * (4f * v + t)) * 0.1f;
        float s = r * Mathf.Cos(pi * 0.5f * v);
        p.x = s * Mathf.Sin(pi * u);
        p.y = Mathf.Sin(pi * 0.5f * v);
        p.z = s * Mathf.Cos(pi * u);
        return p;
    }
    static Vector3 Vector3Torus(float u, float v, float t)
    {
        Vector3 p;
        float r1 = 0.65f + Mathf.Sin(pi * (6f * u + t)) * 0.1f;
        float r2 = 0.2f + Mathf.Sin(pi * (4f * v + t)) * 0.05f;
        float s = r2 * Mathf.Cos(pi * v) + r1;
        p.x = s * Mathf.Sin(pi * u);
        p.y = r2 * Mathf.Sin(pi * v);
        p.z = s * Mathf.Cos(pi * u);
        return p;
    }

    #region 方形
    static Vector3 Square(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = v;
        p.z = 0f;
        return p;
    }
    #endregion

    #region 圆环形
    static Vector3 Circle(float u, float v, float t)
    {
        Vector3 p;
        p.x = Mathf.Sin(pi * u);
        p.y = 0;
        p.z = Mathf.Cos(pi * u);
        return p;
    }
    #endregion

    #region 圆柱
    static Vector3 Cylinder(float u, float v, float t)
    {
        Vector3 p;
        p.x = Mathf.Sin(pi * u);
        p.y = v;
        p.z = Mathf.Cos(pi * u);
        return p;
    }
    #endregion

    #region 五角星
    static Vector3 FivePointedStar(float u, float v, float t)
    {
        Vector3 p;
        float r = 1f + Mathf.Sin(6f * pi * u) * 0.2f;
        p.x = r * Mathf.Sin(pi * u);
        p.y = v;
        p.z = r * Mathf.Cos(pi * u);
        return p;
    }
    #endregion

    #region 腰形
    static Vector3 CylinderWave(float u, float v, float t)
    {
        Vector3 p;
        float r = 1f + Mathf.Sin(2f * pi * v) * 0.2f;
        p.x = r * Mathf.Sin(pi * u);
        p.y = v;
        p.z = r * Mathf.Cos(pi * u);
        return p;
    }
    #endregion

    #region 桃形
    static Vector3 PeachShaped(float u, float v, float t)
    {
        Vector3 p;
        float r = Mathf.Cos(pi * 0.5f * v);
        p.x = r * Mathf.Sin(pi * u);
        p.y = v;
        p.z = r * Mathf.Cos(pi * u);
        return p;
    }
    #endregion

    #region 球形
    static Vector3 Sphere(float u, float v, float t)
    {
        Vector3 p;
        float r = Mathf.Cos(pi * 0.5f * v);
        p.x = r * Mathf.Sin(pi * u);
        p.y = Mathf.Sin(pi * 0.5f * v);
        p.z = r * Mathf.Cos(pi * u);
        return p;
    }
    #endregion

    #region 球中球
    static Vector3 SphereInSphere(float u, float v, float t)
    {
        Vector3 p;
        float s = Mathf.Cos(pi * v) + 0.5f;
		p.x = s * Mathf.Sin(pi * u);
		p.y = Mathf.Sin(pi * v);
		p.z = s * Mathf.Cos(pi * u);
        return p;
    }
    #endregion

    #region 甜甜圈
    static Vector3 Doughnut(float u, float v, float t)
    {
        Vector3 p;
        float r1 = 1f;
		float r2 = 0.5f;
		float s = r2 * Mathf.Cos(pi * v) + r1;
		p.x = s * Mathf.Sin(pi * u);
		p.y = r2 * Mathf.Sin(pi * v);
		p.z = s * Mathf.Cos(pi * u);
        return p;
    }
    #endregion
}
