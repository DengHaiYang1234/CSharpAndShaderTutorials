using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Game流程控制类
public class Game : PersistableObject
{
    public static Game Instance { get; private set; }

    [Header("序列化对象")]
    [SerializeField]
    public ShapeFactory shapeFactory;
    [Header("序列化管理器")]
    public PersistentStorage storage;

    public float CreationSpeed { get; set; }

    public float DestructionSpeed { get; set; }

    public SpawnZone spawnZoneOfLevel { get; set; }

    [Header("创建")]
    public KeyCode creatKey = KeyCode.C;
    [Header("清除")]
    public KeyCode beginKey = KeyCode.N;
    [Header("保存")]
    public KeyCode saveKey = KeyCode.S;
    [Header("加载")]
    public KeyCode loadKey = KeyCode.L;
    [Header("销毁")]
    public KeyCode destoryKey = KeyCode.X;

    public int LevelCount;

    List<Shape> shapes;

    //版本标记
    const int saveVersion = 3;

    private float creationProgress, destructionProgress;

    private int loadedLevelBuildIndex;

    Random.State mainRandomState;

    //OnEnable在级别更改后也会被调用
    private void OnEnable()
    {
        Instance = this;
    }

    void Start()
    {
        mainRandomState = Random.state;

        shapes = new List<Shape>();

        if (Application.isEditor)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene loadedLevlel = SceneManager.GetSceneAt(i);
                if (loadedLevlel.name.Contains("Level "))
                {
                    SceneManager.SetActiveScene(loadedLevlel);
                    loadedLevelBuildIndex = loadedLevlel.buildIndex;
                    return;
                }
            }
        }
        StartCoroutine(LoadLevel(2));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(creatKey))
            CreatShape();
        else if (Input.GetKeyDown(destoryKey))
            DestoryShape();
        else if (Input.GetKeyDown(beginKey))
            BeginNewGame();
        else if (Input.GetKeyDown(saveKey))
        {
            storage.Save(this, saveVersion);
        }
        else if (Input.GetKeyDown(loadKey))
        {
            BeginNewGame();
            storage.Load(this);
        }
        else
        {
            for (int i = 1; i <= LevelCount; i++)
            {
                if (Input.GetKeyDown(KeyCode.Keypad0 + i))
                {
                    BeginNewGame();
                    StartCoroutine(LoadLevel(i));
                    return;
                }
            }
        }

        creationProgress += Time.deltaTime * CreationSpeed;
        while (creationProgress >= 1f)
        {
            creationProgress -= 1f;
            CreatShape();
        }

        destructionProgress += Time.deltaTime * DestructionSpeed;
        while (destructionProgress >= 1f)
        {
            destructionProgress -= 1f;
            DestoryShape();
        }
    }

    void CreatShape()
    {
        Shape instance = shapeFactory.GetRandom();
        Transform t = instance.transform;
        //返回半径为1的球体内的随机点
        t.localPosition = spawnZoneOfLevel.SpawnPoint;
        //随机旋转值
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);

        //改变颜色HSV
        //https://blog.csdn.net/zgjllf1011/article/details/79391241
        instance.SetColor(Random.ColorHSV(
            hueMin: 0f, hueMax: 1f,
            saturationMin: 0.5f, saturationMax: 1f,
            valueMin: 0.25f, valueMax: 1f,
            alphaMin: 1f, alphaMax: 1f
        ));

        shapes.Add(instance);
    }

    void DestoryShape()
    {
        if (shapes.Count > 0)
        {
            int index = Random.Range(0, shapes.Count);
            shapeFactory.Reclaim(shapes[index]);
            int lastIndex = shapes.Count - 1;
            shapes[index] = shapes[lastIndex];
            shapes.RemoveAt(lastIndex);
        }
    }

    void BeginNewGame()
    {
        Random.state = mainRandomState;
        //随机种子
        int seed = Random.Range(0,int.MaxValue) ^ (int)Time.unscaledDeltaTime;
        
        Random.InitState(seed);

        for (var i = 0; i < shapes.Count; i++)
        {
            shapeFactory.Reclaim(shapes[i]);
        }


        shapes.Clear();
    }

    IEnumerator LoadLevel(int levelBuildIndex)
    {
        enabled = false;
        if (loadedLevelBuildIndex > 0)
        {
            yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);
        }
        //异步加载场景
        yield return SceneManager.LoadSceneAsync(
               levelBuildIndex, LoadSceneMode.Additive
            );
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
        loadedLevelBuildIndex = levelBuildIndex;
        enabled = true;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(shapes.Count);
        writer.Write(Random.state);
        writer.Write(loadedLevelBuildIndex);
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
        if(saveVersion >= 3)
        {
            Random.state = reader.ReadRandomState();
        }
        StartCoroutine(LoadLevel(version < 2 ? 1 : reader.ReadInt()));

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
