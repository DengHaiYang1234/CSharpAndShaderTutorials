using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : GameTileContent 
{
	[SerializeField,Range(1.5f,10.5f)]
	float targetingRange = 1.5f;

	Tower towerPrefab;

	TargetPoint target;

	const int enemyLayerMask = 1 << 8;
	
	public override void GameUpdate()
	{
		if(AcquireTarget())
		{
			Debug.Log("Acquired target!");
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Vector3 position = transform.localPosition;
		position.y += 0.01f;
		Gizmos.DrawWireSphere(position,targetingRange);
		if(target != null)
		{
			Gizmos.DrawLine(position,target.Position);
		}
	}

	bool AcquireTarget()
	{
		Collider[] targets = Physics.OverlapSphere(transform.localPosition,targetingRange,enemyLayerMask);
		if(targets.Length > 0)
		{
			target = targets[0].GetComponent<TargetPoint>();
			Debug.Assert(target != null,"Targeted non-enemy!",targets[0]);
			return true;
		}
		target = null;
		return false;
	}

}
