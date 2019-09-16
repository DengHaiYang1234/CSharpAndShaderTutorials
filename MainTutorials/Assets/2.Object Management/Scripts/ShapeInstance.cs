using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ShapeInstance //检测Shape的唯一性
{
    public Shape Shape { get; private set; }

    //唯一生成ID
    int instanceIdOrSaveIndex;

    public ShapeInstance(Shape shape)
    {
        Shape = shape;
        instanceIdOrSaveIndex = shape.InstanceId;
    }

    public ShapeInstance(int saveIndex)
    {
        Shape = null;
        instanceIdOrSaveIndex = saveIndex;
    }

    //检验当前的obj是否有效，是通过与保存的唯一ID比较
    public bool IsValid
    {
        get
        {
            return Shape && instanceIdOrSaveIndex == Shape.InstanceId;
        }
    }

    public void Resolve()
    {
        if (instanceIdOrSaveIndex >= 0)
        {
            Shape = Game.Instance.GetShape(instanceIdOrSaveIndex);
            instanceIdOrSaveIndex = Shape.InstanceId;
        }
    }

    //implicit：也就是说隐式的将Shape类型转换为ShapeInstance类型
    //implicit：用一个shape隐式的构造一个ShapeInstance
    //explicit： 关键字用于声明必须使用强制转换来调用的用户定义的类型转换运算符
    //explicit ：用一个shape显式的构造一个ShapeInstance
    public static implicit operator ShapeInstance(Shape shape)
    {
        return new ShapeInstance(shape);
    }
}
