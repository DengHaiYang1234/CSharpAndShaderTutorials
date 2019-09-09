using UnityEngine;

[CreateAssetMenu(fileName = "GameTileContentFactory", menuName = "Tower Defense/GameTileContentFactory", order = 2)]
public class GameTileContentFactory : GameObjectFactory 
{
	[SerializeField]
	GameTileContent destinationPrefab;

	[SerializeField]
	GameTileContent emptyPrefab;

	[SerializeField]
	GameTileContent wallPrefab;
	[SerializeField]
	GameTileContent spawnPointPrefab;
    [SerializeField]
    Tower[] towerPrefabs;
	//回收
	public void Reclaim(GameTileContent content)
	{
		Debug.Assert(content.OriginFactory == this,"Wrong factory reclaimed!");
		Destroy(content.gameObject);
	}

	GameTileContent Get(GameTileContent prefab)
	{
		GameTileContent instance = CreatGameObjectInstance(prefab);
		instance.OriginFactory = this;
		return instance;
	}

	public GameTileContent Get(GameTileContentType type)
	{
		switch(type)
		{
			case GameTileContentType.Destination:return Get(destinationPrefab);
			case GameTileContentType.Empty:return Get(emptyPrefab);
			case GameTileContentType.Wall:return Get(wallPrefab);
			case GameTileContentType.SpawnPoint:return Get(spawnPointPrefab);
			
		}
		Debug.Assert(false,"Unsupported non-tower type : " + type);
		return null;
	}

    public Tower Get(TowerType type)
    {
        Debug.Assert((int)type < towerPrefabs.Length,"Unsupported tower type!");
        Tower prefab = towerPrefabs[(int) type];
        Debug.Assert(type == prefab.TowerType,"Tower prefab at wrong index!");
        return Get(prefab);
    }

    T Get<T>(T prefab) where T : GameTileContent
    {
        T instance = CreatGameObjectInstance(prefab);
        instance.OriginFactory = this;
        return instance;
    }
}
