using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//对各自的Obj的材质或颜色等操作类
public class Shape : PersistableObject
{
    [SerializeField]
    MeshRenderer[] meshRenders;
    ShapeFactory originFactory;

    //行为抽象集合
    List<ShapeBehavior> behaviorList = new List<ShapeBehavior>();

    public ShapeFactory OriginFactory
    {
        get
        {
            return originFactory;
        }
        set
        {
            if (originFactory == null)
                originFactory = value;
            else
                Debug.LogError("Not allowed to change origin factory!");
        }
    }

    public int ColorCount
    {
        get
        {
            return colors.Length;
        }
    }

    //默认不可以为0，所以找了个数代替
    int shapeId = int.MinValue;

    Color[] colors;

    public int MaterialId { get; private set; }
    public float Age { get; private set; }


    MeshRenderer meshRender;

    static int colorPropertyId;

    static MaterialPropertyBlock sharedPropertyBlock;

    void Awake()
    {
        colors = new Color[meshRenders.Length];
        colorPropertyId = Shader.PropertyToID("_Color");
    }

    //这里不适用FixedUpdate的原因是，FixedUpdate和其他特殊的Unity方法需要额外的开销，这会降低速度。当只有几个活动形状时，这不是问题，但在处理许多形状时，这可能成为性能瓶颈。
    public void GameUpdate()
    {
        Age += Time.deltaTime;
        for (int i = 0; i < behaviorList.Count; i++)
        {
            behaviorList[i].GameUpdate(this);
        }
    }

    //限定添加ShapeBehavior的Behavior组件
    public T AddBehavior<T>() where T : ShapeBehavior, new()
    {
        //不适用monobehavior，使用new
        T behavior = ShapeBehaviorPool<T>.Get();
        behaviorList.Add(behavior);
        return behavior;
    }

    ShapeBehavior AddBehavior(ShapeBehaviorType type)
    {
        switch (type)
        {
            case ShapeBehaviorType.Movement:
                return AddBehavior<MovementShapeBehavior>();
            case ShapeBehaviorType.Rotation:
                return AddBehavior<RotationShapeBehavior>();
        }
        Debug.LogError("Forgot to suppory" + type);
        return null;
    }

    public int ShapeId
    {
        get
        {
            return shapeId;
        }
        set
        {
            if (shapeId == int.MinValue && value != int.MinValue)
                shapeId = value;
            else
                Debug.LogError("Not allowed to change shapeId");
        }
    }

    //统一颜色
    public void SetColor(Color color)
    {
        //this.color = color;
        //使用直接赋值颜色的方式会导致再次实例化一个材质。耗内存
        //meshRender.material.color = color;

        //根据标识符设置当前属性块
        if (sharedPropertyBlock == null)
            sharedPropertyBlock = new MaterialPropertyBlock();
        sharedPropertyBlock.SetColor(colorPropertyId, color);
        for (int i = 0; i < meshRenders.Length; i++)
        {
            colors[i] = color;
            meshRenders[i].SetPropertyBlock(sharedPropertyBlock);
        }
        //meshRender.SetPropertyBlock(sharedPropertyBlock);
    }

    //不统一颜色
    public void SetColor(Color color, int index)
    {
        if (sharedPropertyBlock == null)
        {
            sharedPropertyBlock = new MaterialPropertyBlock();
        }
        sharedPropertyBlock.SetColor(colorPropertyId, color);
        colors[index] = color;
        meshRenders[index].SetPropertyBlock(sharedPropertyBlock);
    }

    public void SetMaterial(Material material, int materialId)
    {
        for (int i = 0; i < meshRenders.Length; i++)
        {
            meshRenders[i].material = material;
        }
        MaterialId = materialId;
    }

    //这一步Save是存储了Transform属性及颜色
    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(colors.Length);
        for (int i = 0; i < colors.Length; i++)
        {
            writer.Write(colors[i]);
        }
        writer.Write(Age);
        writer.Write(behaviorList.Count);
        for (int i = 0; i < behaviorList.Count; i++)
        {
            writer.Write((int)behaviorList[i].BehaviorType);
            behaviorList[i].Save(writer);
        }
    }

    //加载TransForm和颜色
    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        if (reader.Version >= 5)
        {
            LoadColors(reader);
        }
        else
            SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);

        if (reader.Version >= 6)
        {
            Age = reader.ReadFloat();
            int behaviorCount = reader.ReadInt();
            for (int i = 0; i < behaviorCount; i++)
            {
                //AddBehavior((ShapeBehaviorType)reader.ReadInt()).Load(reader);
                ShapeBehavior behavior = ((ShapeBehaviorType)reader.ReadInt()).GetInstance();
                behaviorList.Add(behavior);
                behavior.Load(reader);
            }
        }
        else if (reader.Version >= 4)
        {
            AddBehavior<RotationShapeBehavior>().AngularVelocity = reader.ReadVector();
            AddBehavior<MovementShapeBehavior>().Velocity = reader.ReadVector();
        }
    }

    public void Recycle()
    {
        Age = 0f;
        for (int i = 0; i < behaviorList.Count; i++)
        {
            behaviorList[i].Recycle();
        }
        behaviorList.Clear();
        OriginFactory.Reclaim(this);
    }

    void LoadColors(GameDataReader reader)
    {
        int count = reader.ReadInt();
        int max = count < colors.Length ? count : colors.Length;
        for (int i = 0; i < max; i++)
        {
            SetColor(reader.ReadColor(), i);
        }
        if (count > colors.Length)
        {
            for (int i = 0; i < count; i++)
            {
                reader.ReadColor();
            }
        }
        else if (count < colors.Length)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                SetColor(Color.white, i);
            }
        }
    }


}
