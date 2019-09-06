using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : WarEntity
{
    Vector3 launchPoint, targetPoint, launchVelocity;

    float age, blastRadius, damage;

    public void Initialize(Vector3 launchPoint, Vector3 targetPoint, Vector3 launchVelocity, float blastRadius, float damage)
    {
        this.launchPoint = launchPoint;
        this.targetPoint = targetPoint;
        this.launchVelocity = launchVelocity;
        this.blastRadius = blastRadius;
        this.damage = damage;
    }


    public override bool GameUpdate()
    {
        age += Time.deltaTime;
        Vector3 p = launchPoint + launchVelocity * age;
        p.y -= 0.5f * 9.18f * age * age;//抛物线轨迹，最终落地

        if (p.y <= 0f)
        {
            // TargetPoint.FillBuffer(targetPoint, blastRadius);
            // for (int i = 0; i < TargetPoint.BufferedCount; i++)
            // {
			// 	//结算伤害
            //     TargetPoint.GetBuffered(i).Enemy.ApplyDamage(damage);
            // }

			TowerGame.SpawnExplosion().Initialize(targetPoint,blastRadius,damage);
            OriginFactory.Reclaim(this);
            return false;
        }
		
        transform.localPosition = p;

        //炮弹与轨道对齐	
        Vector3 d = launchVelocity;
        d.y -= 9.81f * age;
        transform.localRotation = Quaternion.LookRotation(d);

		TowerGame.SpawnExplosion().Initialize(p,0.1f);
        return true;
    }
}
