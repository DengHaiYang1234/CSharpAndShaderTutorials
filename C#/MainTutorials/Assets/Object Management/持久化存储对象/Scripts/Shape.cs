using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//对各自的Obj的材质或颜色等操作类
public class Shape : PersistableObject
{
    [SerializeField]
    MeshRenderer[] meshRenders;
    public Vector3 AngularVelocity { get; set; }

    public Vector3 Velocity { get; set; }

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
        transform.Rotate(AngularVelocity * Time.deltaTime);
        transform.localPosition += Velocity * Time.deltaTime;
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
            //必定统一颜色，若不统一，那么赋予其它材质
            meshRenders[i].material = material;
        }
        //meshRender.material = material;
        MaterialId = materialId;
    }

    //这一步Save是存储了Transform属性及颜色
    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        for (int i = 0; i < colors.Length; i++)
        {
            writer.Write(colors[i]);
        }
        writer.Write(AngularVelocity);
        writer.Write(Velocity);
    }

    //加载TransForm和颜色
    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        if (reader.Version >= 5)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                SetColor(reader.ReadColor(),i);
            }
        }
        else
            SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);

        AngularVelocity = reader.Version >= 4 ? reader.ReadVector() : Vector3.zero;

        Velocity = reader.Version >= 4 ? reader.ReadVector() : Vector3.zero;
    }


}
