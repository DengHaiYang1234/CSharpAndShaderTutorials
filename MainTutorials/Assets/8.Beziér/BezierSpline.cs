﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BezierSpline : MonoBehaviour
{
    [HideInInspector]
    [SerializeField]
    private Vector3[] points;

    [HideInInspector]
    [SerializeField]
    private BezierControlPointMode[] modes;//曲线

    [HideInInspector]
    [SerializeField]
    private bool loop;

    public int ControlPointCount
    {
        get { return points.Length; }
    }

    public Vector3 GetControlPonit(int index)
    {
        return points[index];
    }

    public void SetControlPoint(int index, Vector3 point)
    {
        if (index % 3 == 0)//起点
        {
            Vector3 delta = point - points[index];
            if (loop)
            {
                if (index == 0)
                {
                    points[1] += delta;
                    points[points.Length - 2] += delta;
                    points[points.Length - 1] = point;
                }
                else if (index == points.Length - 1)
                {
                    points[0] = point;
                    points[1] += delta;
                    points[index - 1] += delta;
                }
                else
                {
                    points[index - 1] += delta;
                    points[index + 1] += delta;
                }
            }
            else
            {
                if (index > 0)
                {
                    points[index - 1] += delta;
                }

                if (index + 1 < points.Length)
                {
                    points[index + 1] += delta;
                }
            }

        }

        points[index] = point;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"> 从0开始 </param>
    /// <returns></returns>
    public BezierControlPointMode GetControlPointMode(int index)
    {
        return modes[(index + 1) / 3];
        EnforceMode(index);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index">  从0开始  </param>
    /// <param name="mode"></param>
    public void SetControlPointMode(int index, BezierControlPointMode mode)
    {
        int modeIndex = (index + 1) / 3;
        modes[modeIndex] = mode;
        if (loop)//首尾相连
        {
            if (modeIndex == 0)
            {
                modes[modes.Length - 1] = mode;
            }
            else if (modeIndex == modes.Length - 1)
                modes[0] = mode;
        }
        EnforceMode(index);
    }

    /// <summary>
    /// 在固定起点和终点的同时（剔除），选出一点左右位置都满足的情况。例如：0,1,2,3,4,5,6,7.满足条件的是2,3,4
    /// 选取中间点，调整相反点
    /// </summary>
    /// <param name="index"></param>
    void EnforceMode(int index)
    {
        int modeIndex = (index + 1) / 3;
        BezierControlPointMode mode = modes[modeIndex];
        //第一段和最后一段三阶曲线不做处理
        if (mode == BezierControlPointMode.Free || modeIndex == 0 || modeIndex == modes.Length - 1)
            return;

        //各三阶曲线起点
        int middleIndex = modeIndex * 3;
        //fixedIndex：
        int fixedIndex, enforcedIndex;
        if (index <= middleIndex)//位于第一阶段三阶曲线内的点，例如2,3
        {
            fixedIndex = middleIndex - 1; //取前一点
            if (fixedIndex < 0)
            {
                fixedIndex = points.Length - 2;
            }
            enforcedIndex = middleIndex + 1;
            if (enforcedIndex >= points.Length)
            {
                enforcedIndex = 1;
            }
        }
        else
        {
            fixedIndex = middleIndex + 1;
            if (fixedIndex >= points.Length)
                fixedIndex = 1;
            enforcedIndex = middleIndex - 1;
            if (enforcedIndex < 0)
                enforcedIndex = points.Length - 1;
        }

        Vector3 middle = points[middleIndex];
        Vector3 enforceedTangent = middle - points[fixedIndex];
        if (mode == BezierControlPointMode.Aligned)
            enforceedTangent = enforceedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
        points[enforcedIndex] = middle + enforceedTangent;
    }

    public bool Loop
    {
        get { return loop; }
        set
        {
            loop = value;
            if (value == true)
            {
                modes[modes.Length - 1] = modes[0];
                SetControlPoint(0, points[0]);
            }
        }
    }

    public void Reset()
    {
        points = new Vector3[]
        {
            new Vector3(1f,0f,0f),
            new Vector3(2f,0f,0f),
            new Vector3(3f,0f,0f),
            new Vector3(4f,0f,0f)
        };

        modes = new BezierControlPointMode[]
        {
            BezierControlPointMode.Free,
            BezierControlPointMode.Free
        };
    }

    //当前曲线是由几段三阶曲线组成
    public int CurveCount
    {
        get { return (points.Length - 1) / 3; }
    }

    public Vector3 GetPoint(float t)
    {
        int i;
        if (t >= 1f)//已经到达终点
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            //t：进入第几阶段的三阶曲线。例如CurveCount=3.表示有3段三阶曲线，t>0.33时（i = 1），进入第二段三阶曲线，t>0.66（i=2）时，即进入第三阶段三阶曲线
            t = Mathf.Clamp01(t) * CurveCount;
            //i：可以理解为第几阶段三阶曲线的索引
            i = (int)t;
            t -= i;
            i *= 3;
        }

        //坐标变换至世界空间
        return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
    }


    public Vector3 GetVelocity(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t)) -
               transform.position;
    }

    //方向
    public Vector3 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }








    public void AddCurve()
    {
        Vector3 point = points[points.Length - 1];
        Array.Resize(ref points, points.Length + 3);
        point.x += 1f;
        points[points.Length - 3] = point;
        point.x += 1f;
        points[points.Length - 2] = point;
        point.x += 1f;
        points[points.Length - 1] = point;

        Array.Resize(ref modes, modes.Length + 1);
        modes[modes.Length - 1] = modes[modes.Length - 2];

        EnforceMode(points.Length - 4);

        if (loop)
        {
            points[points.Length - 1] = points[0];
            modes[modes.Length - 1] = modes[0];
            EnforceMode(0);
        }
    }

}
