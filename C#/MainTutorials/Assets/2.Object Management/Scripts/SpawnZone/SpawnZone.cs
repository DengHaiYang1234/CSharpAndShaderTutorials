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

        [System.Serializable]
        public struct SatelliteConfiguration
        {
            public IntRange amount;
            [FloatRangeSlider(0.1f, 1f)] public FloatRange relativeScale;

            public FloatRange orbitRadius;

            public FloatRange orbitFrequency;

            public bool uniformLifecycles;
        }

        public SatelliteConfiguration satellite;

        [System.Serializable]
        public struct LifecycleConfiguration
        {
            [FloatRangeSlider(0f, 2f)]
            public FloatRange growingDuration;
            [FloatRangeSlider(0f,100f)]
            public FloatRange adultDuration;
            [FloatRangeSlider(0f,2f)]
            public FloatRange dyingDuration;

            public Vector3 RandomDurations
            {
                get
                {
                    return new Vector3
                    (
                        growingDuration.RandomValueInRange,
                        adultDuration.RandomValueInRange,
                        dyingDuration.RandomValueInRange
                    );
                }
            }
        }

        public LifecycleConfiguration lifecycle;
    }

    [SerializeField]
    SpawnConfiguration spawnConfig;

    public virtual void SpawnShapes()
    {
        int factoryIndex = Random.Range(0, spawnConfig.factories.Length);
        Shape shape = spawnConfig.factories[factoryIndex].GetRandom();

        Transform t = shape.transform;
        t.localPosition = SpawnPoint;
        //随机旋转值
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * spawnConfig.scale.RandomValueInRange;

        SetupColor(shape);

        //float growingDuration = spawnConfig.lifecycle.growingDuration.RandomValueInRange;

        Vector3 lifecycleConfigurations = spawnConfig.lifecycle.RandomDurations;

        int satelliteCount = spawnConfig.satellite.amount.RandomValueInRange;
        for (int i = 0; i < satelliteCount; i++)
        {
            CreatSatelliterFor(shape,spawnConfig.satellite.uniformLifecycles ? lifecycleConfigurations : spawnConfig.lifecycle.RandomDurations);
        }
        
        //if (spawnConfig.uniformColor)
        //{
        //    shape.SetColor(spawnConfig.color.RandomInRange);
        //}
        //else
        //{
        //    for (int i = 0; i < shape.ColorCount; i++)
        //    {
        //        shape.SetColor(spawnConfig.color.RandomInRange, i);
        //    }
        //}

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
            movment.Velocity = GetDirectionVector(spawnConfig.movementDirection, t) * speed;
        }

        SetupOscillation(shape);
        CreatSatelliterFor(shape,lifecycleConfigurations);
        //return shape;
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
        if (amplitude == 0f || frequency == 0f)
        {
            return;
        }

        var oscillation = shape.AddBehavior<OscillationShapeBehavior>();
        oscillation.Offset = GetDirectionVector(spawnConfig.oscillationDirection, shape.transform) * amplitude;
        oscillation.Frequency = frequency;
    }

    void SetupColor(Shape shape)
    {
        if (spawnConfig.uniformColor)
            shape.SetColor(spawnConfig.color.RandomInRange);
        else
        {
            for (int i = 0; i < shape.ColorCount; i++)
            {
                shape.SetColor(spawnConfig.color.RandomInRange, i);
            }
        }
    }

    void CreatSatelliterFor(Shape focalShape, Vector3 lifecycleConfigurations)
    {
        int factoryIndex = Random.Range(0, spawnConfig.factories.Length);
        Shape shape = spawnConfig.factories[factoryIndex].GetRandom();
        Transform t = shape.transform;
        t.localRotation = Random.rotation;
        t.localScale = focalShape.transform.localScale * 0.5f;
        SetupColor(shape);
        shape.AddBehavior<SatelliteShapeBehavior>().Initialize(shape, focalShape,
            spawnConfig.satellite.orbitRadius.RandomValueInRange, spawnConfig.satellite.orbitFrequency.RandomValueInRange);

        SetupLifecycle(shape, lifecycleConfigurations);
    }

    void SetupLifecycle(Shape shape, Vector3 duraiton)
    {
        if (duraiton.x > 0f)
        {
            if(duraiton.x > 0f)
            {
                if(duraiton.y > 0f || duraiton.z > 0f)
                {
                    shape.AddBehavior<LifecycleShapeBehavior>().Initialize(shape,duraiton.x,duraiton.y,duraiton.z);
                }
            }
            else
            {
                shape.AddBehavior<GrowingShapeBehavior>().Initialize(shape,duraiton.x);
            }
        }
        else if(duraiton.y > 0f)
        {
            shape.AddBehavior<LifecycleShapeBehavior>().Initialize(shape,duraiton.x,duraiton.y,duraiton.z);
        }
        else if(duraiton.z > 0f)
        {
            shape.AddBehavior<DyingShapeBehavior>().Initialize(shape,duraiton.z);
        }
    }
}
