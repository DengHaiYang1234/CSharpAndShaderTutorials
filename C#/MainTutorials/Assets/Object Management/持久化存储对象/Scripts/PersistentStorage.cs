using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


//类似序列化存储的桥接类
public class PersistentStorage : MonoBehaviour
{
    string savePath;

    // Use this for initialization
    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "saveFile");
    }
	
    public void Save(PersistableObject o,int version)
    {
        using (BinaryWriter write = new BinaryWriter(File.Open(savePath, FileMode.Create)))
        {
            write.Write(-version);
            o.Save(new GameDataWriter(write));
        }
    }


    public void Load(PersistableObject o)
    {
        //using (BinaryReader reader = new BinaryReader(File.Open(savePath, FileMode.Open)))
        //{
        //    o.Load(new GameDataReader(reader,-reader.ReadInt32()));
        //}

        //异步加载
        byte[] data = File.ReadAllBytes(savePath);
        var reader = new BinaryReader(new MemoryStream(data));
        o.Load(new GameDataReader(reader, -reader.ReadInt32()));
    }
}
