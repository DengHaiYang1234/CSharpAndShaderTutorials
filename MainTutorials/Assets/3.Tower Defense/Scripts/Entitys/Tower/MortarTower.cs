using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarTower : Tower
{
    [SerializeField, Range(0.5f, 2f)] private float shotsPerSecond = 1f;

    [SerializeField] private Transform mortar;

    [SerializeField,Range(0.5f,3f)]
    float shellBlastRadius = 1f;
    [SerializeField,Range(1f,100f)]
    float shellDamage = 10f;

    float launchSpeed;

    float launchProgress;

    public override TowerType TowerType
    {
        get { return TowerType.Mortar; }
    }

    void Awake()
    {
        OnValidate();
    }

    public override void GameUpdate()
    {
        launchProgress += shotsPerSecond * Time.deltaTime;
        while (launchProgress >= 1f)
        {
            TargetPoint target;
            bool acquire = AcquireTarget(out target);
            if (acquire)
            {
                Launch(target);
                launchProgress -= 1f;
            }
            else
            {
                launchProgress = 0.999f;
            }
        }
    }

    //计算攻击轨迹
    public void Launch(TargetPoint target)
    {
        //起始位置
        Vector3 launchPoint = mortar.position;
        //终点
        Vector3 targetPoint = target.Position;

        targetPoint.y = 0f;
        Vector2 dir;

        dir.x = targetPoint.x - launchPoint.x;
        dir.y = targetPoint.z - launchPoint.z;

        float x = dir.magnitude;//模长
        float y = -launchPoint.y;

        dir /= x;//求单位向量

        //****************公式参考 https://catlikecoding.com/unity/tutorials/tower-defense/ballistics/  ****************
        float g = 9.81f;
        float s = launchSpeed;
        float s2 = s * s;

        float r = s2 * s2 - g * (g * x * x + 2f * y * s2);
        Debug.Assert(r >= 0f, "Launch velocity insufficient for range!");
        float tanTheta = (s2 + Mathf.Sqrt(r)) / (g * x);
        float cosTheta = Mathf.Cos(Mathf.Atan(tanTheta));
        float sinTheta = cosTheta * tanTheta;
        //****************************************************************************************************************

        mortar.localRotation = Quaternion.LookRotation(new Vector3(dir.x, tanTheta, dir.y));

        //实例化导弹
        TowerGame.SpawnShell().Initialize(launchPoint, targetPoint, new Vector3(s * cosTheta * dir.x, s * sinTheta, s * cosTheta * dir.y),
                                            shellBlastRadius,shellDamage);

        
        //==============画线可视化============
        // Vector3 prev = launchPoint, next;
        // for (int i = 1; i <= 10; i++)
        // {
        //     float t = i / 10f;
        //     //x轴上移动的距离：速度*时间
        //     float dx = s * cosTheta * t;
        //     //y轴上抛物线移动的距离：速度*时间 - 0.5 * g * t * t
        //     float dy = s * sinTheta * t - 0.5f * g * t * t;
        //     next = launchPoint + new Vector3(dir.x * dx, dy, dir.y * dx);//阶段抛物线终点
        //     Debug.DrawLine(prev, next, Color.blue, 1f);//持续的画出完整抛物线
        //     prev = next;
        // }

        // //三角形的长度等于从塔底指向目标点的二维矢量的长度
        // Debug.DrawLine(launchPoint, targetPoint, Color.yellow, 1f);
        // //计算在x和z轴上的偏移，与上面黄线构成三角形
        // Debug.DrawLine(new Vector3(launchPoint.x, 0.01f, launchPoint.z),
        //                 new Vector3(launchPoint.x + dir.x * x, 0.01f, launchPoint.z + dir.y * x), Color.white, 1f);
    }


    void OnValidate()
    {
        float x = targetingRange + 0.25001f;
        float y = -mortar.position.y;
        launchSpeed = Mathf.Sqrt(9.81f * (y + Mathf.Sqrt(x * x + y * y)));
    }


}
