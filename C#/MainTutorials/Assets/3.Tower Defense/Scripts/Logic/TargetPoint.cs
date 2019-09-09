using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPoint : MonoBehaviour
{
    const int enemyLayerMask = 1 << 8;

    private static Collider[] buffer = new Collider[100];

    public static int BufferedCount { get; private set; }

    public Enemy Enemy { get; private set; }

    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }

    void Awake()
    {
        Enemy = transform.root.GetComponent<Enemy>();
        Debug.Assert(Enemy != null, "Target point without Enemy root!", this);
        Debug.Assert(GetComponent<SphereCollider>() != null, "Target point without sphere collider!", this);
        Debug.Assert(gameObject.layer == 8, "Target point on wrong layer", this);
    }

    public static TargetPoint RandomBuffered
    {
        get
        {
            return GetBuffered(Random.Range(0, BufferedCount));
        }
    }

    //缓存被攻击对象集合
    public static bool FillBuffer(Vector3 postition, float range)
    {
        Vector3 top = postition;
        top.y += 3f;
        //个人理解：在postition，top，半径为range形成的Capsule中，在enemyLayerMask层所碰到的物体都缓存在buffer中,返回值获取碰撞到的物体数量
        BufferedCount = Physics.OverlapCapsuleNonAlloc(postition, top, range, buffer, enemyLayerMask);
        return BufferedCount > 0;
    }
    //获取攻击对象
    public static TargetPoint GetBuffered(int index)
    {
        var target = buffer[index].GetComponent<TargetPoint>();
        Debug.Assert(target != null, "Targeted non-enemy!", buffer[0]);
        return target;
    }

}
