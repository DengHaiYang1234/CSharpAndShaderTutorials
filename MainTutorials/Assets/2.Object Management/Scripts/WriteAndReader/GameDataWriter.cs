using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameDataWriter
{
    BinaryWriter write;

    //用于保存随机数的完整内部状态的可序列化结构
    public void Write(Random.State value)
    {
        write.Write(JsonUtility.ToJson(value));
    }

    public GameDataWriter(BinaryWriter write)
    {
        this.write = write;
    }

    public void Write(float value)
    {
        write.Write(value);
    }

    public void Write(int value)
    {
        write.Write(value);
    }

    public void Write(Quaternion value)
    {
        write.Write(value.x);
        write.Write(value.y);
        write.Write(value.z);
        write.Write(value.w);
    }

    public void Write(Vector3 value)
    {
        write.Write(value.x);
        write.Write(value.y);
        write.Write(value.z);
    }

    public void Write(Color color)
    {
        write.Write(color.r);
        write.Write(color.g);
        write.Write(color.b);
        write.Write(color.a);
    }

    public void Write(ShapeInstance value)
    {
        write.Write(value.IsValid ? value.Shape.SaveIndex : -1);
    }

}
