using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PersistentStorage : MonoBehaviour
{
    string savePath;

    // Use this for initialization
    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "saveFile");
    }
	
    public void Save(PersistableObject o)
    {
        using (BinaryWriter write = new BinaryWriter(File.Open(savePath, FileMode.Create)))
        {
            o.Save(new GameDataWriter(write));
        }
    }

    public void Load(PersistableObject o)
    {
        using (BinaryReader reader = new BinaryReader(File.Open(savePath, FileMode.Open)))
        {
            o.Load(new GameDataReader(reader));
        }
    }
}
