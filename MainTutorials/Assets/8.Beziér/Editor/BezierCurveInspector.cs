using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierSpline))]
public class BezierCurveInspector : Editor
{
    private BezierSpline spline;
    private Transform handleTransform;
    private Quaternion handleRotation;
    private const int stepsPerCurve = 10;

    private float directionScale = 0.5f;

    private const float handleSize = 0.04f;
    private const float pickSize = 0.06f;

    private int selectedIndex = -1;

    private void OnSceneGUI()
    {
        spline = target as BezierSpline;
        handleTransform = spline.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = ShowPoint(0);
        //多个三阶曲线合为一段三阶曲线
        for (int i = 1; i < spline.ControlPointCount; i += 3)
        {
            Vector3 p1 = ShowPoint(i);
            Vector3 p2 = ShowPoint(i + 1);
            Vector3 p3 = ShowPoint(i + 2);

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
            //起点推移至终点
            p0 = p3;
        }

        ShowDirections();

    }

    //绘制切线方向
    void ShowDirections()
    {
        Handles.color = Color.green;
        //起点坐标
        Vector3 point = spline.GetPoint(0f);
        //从起点开始的方向。（点的方向）
        Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
        //在曲线上选择多少个点并且表示其方向
        int steps = stepsPerCurve * spline.CurveCount;
        for (int i = 0; i <= steps; i++)
        {
            //i / (float)steps ： 取值范围为0-1.表示从曲线的起点出发到曲线的终点
            point = spline.GetPoint(i / (float)steps);
            Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps) * directionScale);
        }
    }

    private static Color[] modeColors =
    {
        Color.white,
        Color.yellow,
        Color.cyan,
    };

    private Vector3 ShowPoint(int index)
    {
        Vector3 point = handleTransform.TransformPoint(spline.GetControlPonit(index));

        //白点与相机之前的距离
        float size = HandleUtility.GetHandleSize(point);
        if (index == 0)
            size *= 2f;

        Handles.color = modeColors[(int)spline.GetControlPointMode(index)];

        //获取当前选择的点的索引
        if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap))
        {
            selectedIndex = index;
            Repaint();
        }

        if (selectedIndex == index)
        {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
            }
        }
        return point;
    }

    public override void OnInspectorGUI()
    {
        spline = target as BezierSpline;

        EditorGUI.BeginChangeCheck();
        bool loop = EditorGUILayout.Toggle("Loop",spline.Loop);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline,"Toggle Loop");
            EditorUtility.SetDirty(spline);
            spline.Loop = loop;
        }
            
        if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount)
        {
            DrawSelectdPointInspector();
        }

        spline = target as BezierSpline;
        if (GUILayout.Button("Add Curve"))
        {
            Undo.RecordObject(spline, "Add Curve");
            spline.AddCurve();
            EditorUtility.SetDirty(spline);
        }
    }

    //选取的点
    void DrawSelectdPointInspector()
    {
        GUILayout.Label("Selected point");
        EditorGUI.BeginChangeCheck();
        Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetControlPonit(selectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Move Point");
            EditorUtility.SetDirty(spline);
            spline.SetControlPoint(selectedIndex, point);
        }

        EditorGUI.BeginChangeCheck();
        BezierControlPointMode mode =
            (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Change Point Mode");
            spline.SetControlPointMode(selectedIndex, mode);
            EditorUtility.SetDirty(spline);
        }
    }
}
