using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Game流程控制类
public class Game : PersistableObject
{
    [Header("序列化对象")]
    public ShapeFactory shapeFactory;
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

    List<Shape> shapes;
    
    //版本标记
    const int saveVersion = 1;

    void Awake()
    {
        shapes = new List<Shape>();
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
            storage.Save(this,saveVersion);
        }

        if (Input.GetKeyDown(loadKey))
        {
            BeginNewGame();
            storage.Load(this);
        }
    }

    void CreatObject()
    {
        Shape instance = shapeFactory.GetRandom();
        Transform t = instance.transform;
        //返回半径为1的球体内的随机点
        t.localPosition = Random.insideUnitSphere * 5f;
        //随机旋转值
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        //改变颜色HSV
        //https://blog.csdn.net/zgjllf1011/article/details/79391241
        instance.SetColor(Random.ColorHSV(
            hueMin : 0f,hueMax : 1f,
            saturationMin : 0.5f,saturationMax : 1f,
            valueMin : 0.25f,valueMax : 1f,
            alphaMin : 1f,alphaMax : 1f
        ));

        shapes.Add(instance);
    }

    void BeginNewGame()
    {
        foreach (var obj in shapes)
        {
            Destroy(obj.gameObject);
        }


        shapes.Clear();
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(shapes.Count);
        for (int i = 0; i < shapes.Count; i++)
        {
            writer.Write(shapes[i].ShapeId);
            writer.Write(shapes[i].MaterialId);
            shapes[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int version = reader.Version;
        if (version > saveVersion)
        {
            Debug.LogError("Unsupported future save version" + version);
            return;
        }

        int count = version <= 0 ? -version : reader.ReadInt();

        for (int i = 0; i < count; i++)
        {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFactory.Get(shapeId);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }
}
