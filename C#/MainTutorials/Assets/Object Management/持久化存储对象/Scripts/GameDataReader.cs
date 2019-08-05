using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameDataReader : MonoBehaviour
{
    BinaryReader reader;

	public int Version{get;}

    public GameDataReader(BinaryReader reader, int version)
    {
        this.reader = reader;
        this.Version = version;
    }


    public float ReadFloat()
    {
        return reader.ReadSingle();
    }

    public int ReadInt()
    {
        return reader.ReadInt32();
    }

    public Quaternion ReadQuaternion()
    {
        Quaternion q;
        q.x = ReadFloat();
        q.y = ReadFloat();
        q.z = ReadFloat();
        q.w = ReadFloat();
        return q;
    }

    public Vector3 ReadVector()
    {
        Vector3 v;
        v.x = ReadFloat();
        v.y = ReadFloat();
        v.z = ReadFloat();
        return v;
    }

    public Color ReadColor()
    {
        Color v;
        v.r = ReadFloat();
        v.g = ReadFloat();
        v.b = ReadFloat();
        v.a = ReadFloat();
        return v;
    }

}
