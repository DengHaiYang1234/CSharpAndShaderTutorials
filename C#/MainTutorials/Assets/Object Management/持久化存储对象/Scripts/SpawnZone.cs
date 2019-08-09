using UnityEngine;

//生成各式不同形状
//注：Random生成的各种形状的点位都是伪随机。如果重新加载就不一定跟上次一样了！！
public abstract class SpawnZone : PersistableObject
{
    public abstract Vector3 SpawnPoint { get; }

    [System.Serializable]
    public struct SpawnConfiguration
    {
        public enum SpawnMovementDirection
        {
            Forwrad,
            Upward,
            OutWard,
            Random,
        }

        public SpawnMovementDirection movementDirection;

        public FloatRange speed;

        public FloatRange angularSpeed;

        public FloatRange scale;

        public ColorRangeHSV color;

        public bool uniformColor;
    }

    [SerializeField]
    SpawnConfiguration spawnConfig;

    public virtual void ConfigureSpawn(Shape shape)
    {
        Transform t = shape.transform;

        t.localPosition = SpawnPoint;
        //随机旋转值
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * spawnConfig.scale.RandomValueInRange;

        if(spawnConfig.uniformColor)
        {
            shape.SetColor(spawnConfig.color.RandomInRange);
        }
        else
        {
            Debug.LogError("=============");
            for(int i = 0; i < shape.ColorCount;i++)
            {
                Debug.Log("color:" + spawnConfig.color.RandomInRange);
                shape.SetColor(spawnConfig.color.RandomInRange,i);
            }
            Debug.LogError("=============");
        }

        //改变颜色HSV
        //https://blog.csdn.net/zgjllf1011/article/details/79391241
        //shape.SetColor(Random.ColorHSV(
        //    hueMin: 0f, hueMax: 1f,
        //    saturationMin: 0.5f, saturationMax: 1f,
        //    valueMin: 0.25f, valueMax: 1f,
        //    alphaMin: 1f, alphaMax: 1f
        //));
        Debug.Log("Color:" + spawnConfig.color.RandomInRange);
        shape.SetColor(spawnConfig.color.RandomInRange);
        shape.AngularVelocity = Random.onUnitSphere * spawnConfig.angularSpeed.RandomValueInRange;

        Vector3 direction;
        switch (spawnConfig.movementDirection)
        {
            case SpawnConfiguration.SpawnMovementDirection.Upward:
                direction = transform.up;
                break;
            case SpawnConfiguration.SpawnMovementDirection.OutWard:
                direction = (t.localPosition - transform.position).normalized;
                break;
            case SpawnConfiguration.SpawnMovementDirection.Random:
                direction = Random.onUnitSphere;
                break;
            default:
                direction = transform.forward;
                break;
        }

        shape.Velocity = direction * spawnConfig.speed.RandomValueInRange;
    }
}
