using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bezier
{
    //二阶
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        //Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t); 由这个推导而来
        //B(t) = (1 - t) P0 + t P1.
        //B(t) = (1 - t) ((1 - t) P0 + t P1) + t ((1 - t) P1 + t P2).
        //B(t) = (1 - t) * (1 - t) * P0 + 2 * (1 - t) * t * P1 + t * t * P2.
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;

        return oneMinusT * oneMinusT * p0 + 2f * oneMinusT * t * p1 + t * t * p2;
    }

    //二阶切线
    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return 2f * (1f - t) * (p1 - p0) + 2f * t * (p2 - p1);
    }

    //三阶
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        //Vector3.Lerp(Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t), Vector3.Lerp(Vector3.Lerp(p1, p2, t), Vector3.Lerp(p2, p3, t), t), t);
        //起点p0--控制点p1--控制点p2；起点p1--控制点p2--终点p3；
        //B(t) = (1 - t)^3 * P0 + 3 * (1 - t)^2 * t * P1 + 3 *(1 - t)* t^2 * P2 + t^3*P3

        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * oneMinusT * p0 + 3f * oneMinusT * oneMinusT * t * p1 + 3f * oneMinusT * t * t * p2 + t * t * t * p3;
    }

    //三阶切线
    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        //求导法则别忘了...
        //B'(t) = 3 (1 - t)^2 (P1 - P0) + 6 (1 - t) t (P2 - P1) + 3 t^2 (P3 - P2)
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return 3f * oneMinusT * oneMinusT * (p1 - p0) + 6f * oneMinusT * t * (p2 - p1) + 3f * t * t * (p3 - p2);
    }
}
