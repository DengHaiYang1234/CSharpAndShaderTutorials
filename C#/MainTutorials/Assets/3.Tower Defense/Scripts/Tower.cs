using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : GameTileContent
{
    const int enemyLayerMask = 1 << 8;

    private static Collider[] targetsBuffer = new Collider[100];

    [SerializeField, Range(1.5f, 10.5f)]
    protected float targetingRange = 1.5f;// 半径

    public abstract TowerType TowerType { get; }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.localPosition;
        position.y += 0.01f;
        //以塔楼为中心，targetingRange为半径画圆
        Gizmos.DrawWireSphere(position, targetingRange);
        //if (target != null)
        //{
        //    Gizmos.DrawLine(position, target.Position);
        //}
    }

    //获取目标
    protected bool AcquireTarget(out TargetPoint target)
    {
        Vector3 a = transform.localPosition;
        Vector3 b = a;
        b.y += 2f;

        //个人理解：在a，b，半径为targetingRange形成的Capsule中，在enemyLayerMask层所碰到的物体都缓存在targetsBuffer中
        int hits = Physics.OverlapCapsuleNonAlloc(a, b, targetingRange, targetsBuffer, enemyLayerMask);

        if (hits > 0)
        {
            //获取碰撞到的目标
            target = targetsBuffer[Random.Range(0, hits)].GetComponent<TargetPoint>();
            Debug.Assert(target != null, "Targeted non-enemy!", targetsBuffer[0]);
            return true;
        }
        target = null;
        return false;
    }

    //是否超出检测半径
    protected bool TrackTraget(ref TargetPoint target)
    {
        if (target == null)
        {
            return false;
        }

        Vector3 a = transform.localPosition;
        Vector3 b = target.Position;
        float x = a.x - b.x;
        float z = a.z - b.z;
        //targetingRange：塔的检测半径 + 0.125倍的Enemy大小
        float r = targetingRange + 0.125f * target.Enemy.Scale;
        //计算在XZ平面，是否超出了检测半径
        if (x * x + z * z > r * r)
        {
            target = null;
            return false;
        }
        return true;
    }



}
