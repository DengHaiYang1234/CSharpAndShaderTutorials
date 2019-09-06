using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WarFactory : GameObjectFactory 
{
	[SerializeField]
	Shell shellPrefab;

	[SerializeField]
	Explosion explosionPrefab;

	public Explosion Explosion
	{
		get
		{
			return Get(explosionPrefab);
		}
	}
	
	public Shell Shell
	{
		get
		{
			return Get(shellPrefab);
		}
	}

	T Get<T>(T prefab) where T : WarEntity
	{
		T instance = CreatGameObjectInstance(prefab);
		instance.OriginFactory =this;
		return instance;
	}

	public void Reclaim(WarEntity entity)
	{
		Debug.Assert(entity.OriginFactory == this,"Wrong factory reclaimed!");
		Destroy(entity.gameObject);
	}
}
