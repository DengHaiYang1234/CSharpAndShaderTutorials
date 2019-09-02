
using UnityEngine;

[CreateAssetMenu]
public class EnemyFactory : GameObjectFactory
{
	[SerializeField]
	Enemy prefab;

	public Enemy Get()
	{
		Enemy instance = CreatGameObjectInstance(prefab);
		instance.OriginFactory = this;
		return instance;
	}


	public void Reclaim(Enemy enemy)
	{
		Debug.Assert(enemy.OriginFactory == this,"Wring factory reclaimed!");
		Destroy(enemy.gameObject);
	}

}
