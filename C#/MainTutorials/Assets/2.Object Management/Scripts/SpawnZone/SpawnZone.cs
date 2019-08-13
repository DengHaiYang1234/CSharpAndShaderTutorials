using UnityEngine;

//生成各式不同形状
//注：Random生成的各种形状的点位都是伪随机。如果重新加载就不一定跟上次一样了！！
public abstract class SpawnZone : PersistableObject
{
    public abstract Vector3 SpawnPoint { get; }

    [SerializeField, Range(0f, 50f)] private float spawnSpeed;

    private float spawnProgress;

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
        //角速度
        public FloatRange angularSpeed;

        public FloatRange scale;

        public ColorRangeHSV color;

        public bool uniformColor;

        public SpawnMovementDirection oscillationDirection;
        //振荡幅度
        public FloatRange oscillationAmplitude;
        //振荡频率
        public FloatRange oscillationFrequency;

        [System.Serializable]
        public struct SatelliteConfiguration
        {
            public IntRange amount;
            [FloatRangeSlider(0.1f, 1f)] public FloatRange relativeScale;

            public FloatRange orbitRadius;

            public FloatRange orbitFrequency;
            //循环
            public bool uniformLifecycles;
        }

        public SatelliteConfiguration satellite;

        [System.Serializable]
        public struct LifecycleConfiguration
        {
            //生成时间
            [FloatRangeSlider(0f, 2f)]
            public FloatRange growingDuration;
            [FloatRangeSlider(0f,100f)]
            public FloatRange adultDuration;
            //死亡时间
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
        //随机场景
        int factoryIndex = Random.Range(0, spawnConfig.factories.Length);
        //生成prefab在指定的场景下
        Shape shape = spawnConfig.factories[factoryIndex].GetRandom();

        Transform t = shape.transform;
        t.localPosition = SpawnPoint;
        //随机旋转值
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * spawnConfig.scale.RandomValueInRange;

        SetupColor(shape);
        
        Vector3 lifecycleConfigurations = spawnConfig.lifecycle.RandomDurations;

        int satelliteCount = spawnConfig.satellite.amount.RandomValueInRange;

        for (int i = 0; i < satelliteCount; i++)
        {
            CreatSatelliterFor(shape,spawnConfig.satellite.uniformLifecycles ? lifecycleConfigurations : spawnConfig.lifecycle.RandomDurations);
        }
  
        //改变颜色HSV
        //https://blog.csdn.net/zgjllf1011/article/details/79391241
        //shape.SetColor(Random.ColorHSV(
        //    hueMin: 0f, hueMax: 1f,
        //    saturationMin: 0.5f, saturationMax: 1f,
        //    valueMin: 0.25f, valueMax: 1f,
        //    alphaMin: 1f, alphaMax: 1f
        //));

        //添加旋转
        float angularSpeed = spawnConfig.angularSpeed.RandomValueInRange;
        if (angularSpeed != 0f)
        {
            var rotation = shape.AddBehavior<RotationShapeBehavior>();
            rotation.AngularVelocity = Random.onUnitSphere * angularSpeed;
        }

        //根据方向计算速度  添加速度
        float speed = spawnConfig.speed.RandomValueInRange;
        if (speed != 0f)
        {
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

    //振动
    void SetupOscillation(Shape shape)
    {
        float amplitude = spawnConfig.oscillationAmplitude.RandomValueInRange;
        float frequency = spawnConfig.oscillationFrequency.RandomValueInRange;
        if (amplitude == 0f || frequency == 0f)
        {
            return;
        }

        var oscillation = shape.AddBehavior<OscillationShapeBehavior>();
        //幅度
        oscillation.Offset = GetDirectionVector(spawnConfig.oscillationDirection, shape.transform) * amplitude;
        //频率
        oscillation.Frequency = frequency;
    }

    //设置颜色
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

    /// <summary>
    /// 绕目标旋转
    /// </summary>
    /// <param name="focalShape"> 目标obj </param>
    /// <param name="lifecycleConfigurations">  </param>
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

    /// <summary>
    /// 重新
    /// </summary>
    /// <param name="shape"> 绕目标旋转的obj </param>
    /// <param name="duraiton"> x：生成时间 y：持续时间 z：死亡时间 </param>
    void SetupLifecycle(Shape shape, Vector3 duration)
    {
        if (duration.x > 0f)
        {
            if(duration.x > 0f)
            {
                if(duration.y > 0f || duration.z > 0f)
                {
                    shape.AddBehavior<LifecycleShapeBehavior>().Initialize(shape, duration.x, duration.y, duration.z);
                }
            }
            else
            {
                shape.AddBehavior<GrowingShapeBehavior>().Initialize(shape, duration.x);
            }
        }
        else if(duration.y > 0f)
        {
            shape.AddBehavior<LifecycleShapeBehavior>().Initialize(shape, duration.x, duration.y, duration.z);
        }
        else if(duration.z > 0f)
        {
            shape.AddBehavior<DyingShapeBehavior>().Initialize(shape, duration.z);
        }
    }

    private void FixedUpdate()
    {
        spawnProgress += Time.deltaTime*spawnSpeed;
        while (spawnProgress >= 1f)
        {
            spawnProgress -= 1f;
            SpawnShapes();
        }
    }
}
