using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Game : PersistableObject
{
    [Header("序列化对象")]
    public PersistableObject prefab;
    [Header("序列化管理器")]
    public PersistentStorage storage;
    [Header("创建")]
    public KeyCode creatKey = KeyCode.C;
    [Header("清除")]
    public KeyCode beginKey = KeyCode.N;
    [Header("保存")]
    public KeyCode saveKey = KeyCode.S;
    [Header("加载")]
    public KeyCode loadKey = KeyCode.L;

    List<PersistableObject> objs;	
	
    void Awake()
    {
        objs = new List<PersistableObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(creatKey))
            CreatObject();

        if (Input.GetKeyDown(beginKey))
            BeginNewGame();

        if (Input.GetKeyDown(saveKey))
		{
			Debug.Log("1");
			storage.Save(this);
		}
            
        if (Input.GetKeyDown(loadKey))
		{
			BeginNewGame();
			storage.Load(this);
		}
    }

    void CreatObject()
    {
        PersistableObject o = Instantiate(prefab);
        Transform t = o.transform;
		t.localPosition = Random.insideUnitSphere * 5f;
		t.localRotation = Random.rotation;
		t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        objs.Add(o);
    }

    void BeginNewGame()
    {
        foreach (var obj in objs)
        {
            Destroy(obj.gameObject);
        }
        objs.Clear();
    }

    public override void Save(GameDataWriter writer)
    {	
		writer.Write(objs.Count);
		for (int i = 0; i < objs.Count; i++) {
			objs[i].Save(writer);
		}
    }

    public override void Load(GameDataReader reader)
    {
		int count = reader.ReaderInt();
		for (int i = 0; i < count; i++) {
			PersistableObject o = Instantiate(prefab);
			o.Load(reader);
			objs.Add(o);
		}
    }
}
