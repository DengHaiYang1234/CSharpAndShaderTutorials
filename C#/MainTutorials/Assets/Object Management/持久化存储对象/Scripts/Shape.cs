using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//对各自的Obj的材质或颜色等操作类
public class Shape : PersistableObject
{
    //默认不可以为0，所以找了个数代替
    int shapeId = int.MinValue;

    Color color;

    public int MaterialId { get; private set; }

    MeshRenderer meshRender;

    static int colorPropertyId;

    static MaterialPropertyBlock sharedPropertyBlock;

    void Awake()
    {
        meshRender = GetComponent<MeshRenderer>();
        //获取颜色标识符
        colorPropertyId = Shader.PropertyToID("_Color");
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
    
    public void SetColor(Color color)
    {
        this.color = color;
        //使用直接赋值颜色的方式会导致再次实例化一个材质。耗内存
        //meshRender.material.color = color;

        //根据标识符设置当前属性块
        if (sharedPropertyBlock == null)
            sharedPropertyBlock = new MaterialPropertyBlock();
        sharedPropertyBlock.SetColor(colorPropertyId, color);
        meshRender.SetPropertyBlock(sharedPropertyBlock);
    }

    public void SetMaterial(Material material, int materialId)
    {
        meshRender.material = material;
        MaterialId = materialId;
    }

    //这一步Save是存储了Transform属性及颜色
    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(color);
    }

    //加载TransForm和颜色
    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
    }


}
