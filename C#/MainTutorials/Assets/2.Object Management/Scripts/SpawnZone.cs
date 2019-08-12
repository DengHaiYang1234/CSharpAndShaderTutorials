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

        public ShapeFactory[] factories;

        public SpawnMovementDirection movementDirection;

        public FloatRange speed;

        public FloatRange angularSpeed;

        public FloatRange scale;

        public ColorRangeHSV color;

        public bool uniformColor;

        public SpawnMovementDirection oscillationDirection;

        public FloatRange oscillationAmplitude;

        public FloatRange oscillationFrequency;
    }

    [SerializeField]
    SpawnConfiguration spawnConfig;

    public virtual Shape SpawnShape()
    {
        int factoryIndex = Random.Range(0, spawnConfig.factories.Length);
        Shape shape = spawnConfig.factories[factoryIndex].GetRandom();

        Transform t = shape.transform;

        t.localPosition = SpawnPoint;
        //随机旋转值
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * spawnConfig.scale.RandomValueInRange;

        if (spawnConfig.uniformColor)
        {
            shape.SetColor(spawnConfig.color.RandomInRange);
        }
        else
        {
            for (int i = 0; i < shape.ColorCount; i++)
            {
                shape.SetColor(spawnConfig.color.RandomInRange, i);
            }
        }

        //改变颜色HSV
        //https://blog.csdn.net/zgjllf1011/article/details/79391241
        //shape.SetColor(Random.ColorHSV(
        //    hueMin: 0f, hueMax: 1f,
        //    saturationMin: 0.5f, saturationMax: 1f,
        //    valueMin: 0.25f, valueMax: 1f,
        //    alphaMin: 1f, alphaMax: 1f
        //));

        float angularSpeed = spawnConfig.angularSpeed.RandomValueInRange;
        if (angularSpeed != 0f)
        {
            var rotation = shape.AddBehavior<RotationShapeBehavior>();

            rotation.AngularVelocity = Random.onUnitSphere * angularSpeed;
        }

        float speed = spawnConfig.speed.RandomValueInRange;
        if (speed != 0f)
        {
            Vector3 direction;


            var movment = shape.AddBehavior<MovementShapeBehavior>();
            movment.Velocity = GetDirectionVector(spawnConfig.movementDirection,t) * speed;
        }

        SetupOscillation(shape);

        return shape;
    }

    Vector3 GetDirectionVector(SpawnConfiguration.SpawnMovementDirection direction, Transform t)
    {
        switch (direction)
        {
            case SpawnConfiguration.SpawnMovementDirection.Upward:
                return transform.up;
            case SpawnConfiguration.SpawnMovementDirection.OutWard:
                return (t.localPosition - transform.position).normalized;
            case SpawnConfiguration.SpawnMovementDirection.Random:
                return Random.onUnitSphere;
            default:
                return transform.forward;
        }
    }

    //震动
    void SetupOscillation(Shape shape)
    {
        float amplitude = spawnConfig.oscillationAmplitude.RandomValueInRange;
        float frequency = spawnConfig.oscillationFrequency.RandomValueInRange;
        if(amplitude == 0f || frequency == 0f)
        {
            return;
        }

        var oscillation = shape.AddBehavior<OscillationShapeBehavior>();
        oscillation.Offset = GetDirectionVector(spawnConfig.oscillationDirection,shape.transform) * amplitude;
        oscillation.Frequency = frequency;
    }
}
