using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : Tower
{

    [SerializeField, Range(1f, 100f)]
    private float damagePerSecond = 10f;

    [SerializeField]
    private Transform turret, laserBeam;

    private Vector3 laserBeamScale;

    Tower towerPrefab;

    TargetPoint target;

    public override TowerType TowerType
    {
        get { return TowerType.Laser; }
    }
    
    private void Awake()
    {
        laserBeamScale = laserBeam.localScale;
    }

    public override void GameUpdate()
    {
        if (TrackTraget(ref target) || AcquireTarget(out target))
        {
            Shoot();
        }
        else
        {
            laserBeam.localScale = Vector3.zero;
        }
    }

    void Shoot()
    {
        //锁定塔楼和激光的转向
        Vector3 point = target.Position;
        turret.LookAt(point);
        laserBeam.localRotation = turret.localRotation;

        float d = Vector3.Distance(turret.position, point);
        laserBeamScale.z = d;
        laserBeam.localScale = laserBeamScale;
        laserBeam.localPosition = turret.localPosition + 0.5f * d
    * laserBeam.forward;
        //伤害计算
        target.Enemy.ApplyDamage(damagePerSecond * Time.deltaTime);
    }
}
