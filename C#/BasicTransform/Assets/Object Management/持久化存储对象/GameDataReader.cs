using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameDataReader : MonoBehaviour
{
    BinaryReader reader;

    public GameDataReader(BinaryReader reader)
    {
        this.reader = reader;
    }


    public float ReaderFloat()
    {
        return reader.ReadSingle();
    }

    public int ReaderInt()
    {
        return reader.ReadInt32();
    }

	public Quaternion ReadQuaternion()
	{
		Quaternion q;
		q.x = ReaderFloat();
		q.y = ReaderFloat();
		q.z = ReaderFloat();
		q.w = ReaderFloat();
		return q;
	}
	
	public Vector3 ReadVector()
	{
		Vector3 v;
		v.x = ReaderFloat();
		v.y = ReaderFloat();
		v.z = ReaderFloat();
		return v;
	}

}
