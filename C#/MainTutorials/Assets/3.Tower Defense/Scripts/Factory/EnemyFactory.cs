
using UnityEngine;

[CreateAssetMenu]
public class EnemyFactory : GameObjectFactory
{
	[SerializeField]
	Enemy prefab;

	[SerializeField,TowerFloatRangeSlider(0.5f,2f)]
	TowerFloatRange scale = new TowerFloatRange(1f);

	[SerializeField,TowerFloatRangeSlider(-0.4f,0.4f)]
	TowerFloatRange pathOffest = new TowerFloatRange(0f);

	[SerializeField,TowerFloatRangeSlider(0.2f,5f)]
	TowerFloatRange speed = new TowerFloatRange(1f);

	public Enemy Get()
	{
		Enemy instance = CreatGameObjectInstance(prefab);
		instance.OriginFactory = this;
		instance.Initialize(scale.RandomValueInRange,pathOffest.RandomValueInRange,speed.RandomValueInRange);
		return instance;
	}


	public void Reclaim(Enemy enemy)
	{
		Debug.Assert(enemy.OriginFactory == this,"Wring factory reclaimed!");
		Destroy(enemy.gameObject);
	}

}
