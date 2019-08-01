using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameDataWriter
{
    BinaryWriter write;

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

}
