using UnityEngine;

[CreateAssetMenu(fileName = "EnemyFactory", menuName = "Tower Defense/EnemyFactory", order = 2)]
public class EnemyFactory : GameObjectFactory
{
    [System.Serializable]
    class EnemyConfig
    {
        public Enemy prefab;

        [TowerFloatRangeSlider(0.5f, 2f)]
        public TowerFloatRange scale = new TowerFloatRange(1f);

        [TowerFloatRangeSlider(0.2f, 5f)]
        public TowerFloatRange speed = new TowerFloatRange(1f);

        [TowerFloatRangeSlider(-0.4f, 0.4f)]
        public TowerFloatRange pathOffset = new TowerFloatRange(0f);

        [TowerFloatRangeSlider(10f, 1000f)]
        public TowerFloatRange health = new TowerFloatRange(100f);
    }

    [SerializeField]
    EnemyConfig small, medium, large;

    EnemyConfig GetConfig(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Small: return small;
            case EnemyType.Medium: return medium;
            case EnemyType.Large: return large;
        }
        Debug.Assert(false, "Unsupported enemy type!");
        return null;
    }

    public Enemy Get(EnemyType type = EnemyType.Medium)
    {
        EnemyConfig config = GetConfig(type);
        Enemy instance = CreatGameObjectInstance(config.prefab);
        instance.OriginFactory = this;
        instance.Initialize(config.scale.RandomValueInRange, config.pathOffset.RandomValueInRange, config.speed.RandomValueInRange, config.health.RandomValueInRange);
        return instance;
    }


    public void Reclaim(Enemy enemy)
    {
        Debug.Assert(enemy.OriginFactory == this, "Wring factory reclaimed!");
        Destroy(enemy.gameObject);
    }

}
