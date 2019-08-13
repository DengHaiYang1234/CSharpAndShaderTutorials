using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Game流程控制类
public class Game : PersistableObject
{
    public static Game Instance { get; private set; }

    // [Header("序列化对象")]
    // [SerializeField]
    //public ShapeFactory shapeFactory;

    [Header("序列化管理器")]
    public PersistentStorage storage;


    [SerializeField] private Slider creationSpeedSlider;
    [SerializeField] private Slider destructionSpeedSlider;

    [SerializeField] ShapeFactory[] shapeFactories;

    [SerializeField] float destoryDuration;

    //创建速度
    public float CreationSpeed { get; set; }
    //销毁速度
    public float DestructionSpeed { get; set; }

    public SpawnZone spawnZoneOfLevel { get; set; }

    [SerializeField]
    private bool reseedOnLoad;

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

    List<ShapeInstance> killList, markAsDyingList;

    //每个版本代表不同的意思。比如4就是存储的旋转
    const int saveVersion = 6;

    //创建于销毁的控制条
    private float creationProgress, destructionProgress;

    private int loadedLevelBuildIndex;

    Random.State mainRandomState;

    bool inGameUpdateLoop;

    int dyingShapeCount;

    void Start()
    {
        mainRandomState = Random.state;

        shapes = new List<Shape>();
        killList = new List<ShapeInstance>();
        markAsDyingList = new List<ShapeInstance>();

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

    void OnEnable()
    {
        Instance = this;
        if (shapeFactories[0].FactoryId != 0)
        {
            for (int i = 0; i < shapeFactories.Length; i++)
            {
                shapeFactories[i].FactoryId = i;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(creatKey))
            GameLevel.Current.SpawnShape();
        else if (Input.GetKeyDown(destoryKey))
            DestoryShape();
        else if (Input.GetKeyDown(beginKey))
        {
            BeginNewGame();
            StartCoroutine(LoadLevel(loadedLevelBuildIndex));
        }
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
    }

    //时间平滑  Update时间不稳定
    private void FixedUpdate()
    {
        inGameUpdateLoop = true;
        for (int i = 0; i < shapes.Count; i++)
        {
            shapes[i].GameUpdate();
        }
        inGameUpdateLoop = false;
        creationProgress += Time.deltaTime * CreationSpeed;
        while (creationProgress >= 1f)
        {
            creationProgress -= 1f;
            GameLevel.Current.SpawnShape();
        }

        destructionProgress += Time.deltaTime * DestructionSpeed;
        while (destructionProgress >= 1f)
        {
            destructionProgress -= 1f;
            DestoryShape();
        }

        int limit = GameLevel.Current.PopulationLimit;
        if (limit > 0)
        {
            while (shapes.Count - dyingShapeCount > limit)
                DestoryShape();
        }

        if (killList.Count > 0)
        {
            for (int i = 0; i < killList.Count; i++)
            {
                Debug.Log("IsValid:" + killList[i].IsValid);
                Debug.Log("name:" + killList[i].Shape.name);
                Debug.Log("====================================:");
                if (killList[i].IsValid)
                {
                    KillImmediately(killList[i].Shape);
                }
            }

            killList.Clear();
        }

        if (markAsDyingList.Count > 0)
        {
            for (int i = 0; i < markAsDyingList.Count; i++)
            {
                if (markAsDyingList[i].IsValid)
                {
                    MarkAsDyingImmediately(markAsDyingList[i].Shape);
                }
                markAsDyingList.Clear();
            }
        }
    }

    void DestoryShape()
    {
        if (shapes.Count - dyingShapeCount > 0)
        {
            Shape shape = shapes[Random.Range(dyingShapeCount, shapes.Count)];
            if(destoryDuration<=0f)
            {
                KillImmediately(shape);
            }
            else
            {
                shape.AddBehavior<DyingShapeBehavior>().Initialize(shape,destoryDuration);
            }
        }
    }

    void BeginNewGame()
    {
        creationSpeedSlider.value = CreationSpeed = 0;
        destructionSpeedSlider.value = DestructionSpeed = 0;

        Random.state = mainRandomState;
        //随机种子
        int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledDeltaTime;
        mainRandomState = Random.state;
        Random.InitState(seed);

        for (var i = 0; i < shapes.Count; i++)
        {
            shapes[i].Recycle();
        }


        shapes.Clear();
        dyingShapeCount = 0;
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
        writer.Write(CreationSpeed);
        writer.Write(creationProgress);
        writer.Write(DestructionSpeed);
        writer.Write(destructionProgress);
        writer.Write(loadedLevelBuildIndex);
        GameLevel.Current.Save(writer);
        for (int i = 0; i < shapes.Count; i++)
        {
            writer.Write(shapes[i].OriginFactory.FactoryId);
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

        StartCoroutine(LoadGame(reader));
    }

    IEnumerator LoadGame(GameDataReader reader)
    {
        int version = reader.Version;

        int count = version <= 0 ? -version : reader.ReadInt();
        if (saveVersion >= 3)
        {
            Random.State state = reader.ReadRandomState();
            if (!reseedOnLoad)
                Random.state = state;

            creationSpeedSlider.value = CreationSpeed = reader.ReadFloat();
            creationProgress = reader.ReadFloat();
            destructionSpeedSlider.value = DestructionSpeed = reader.ReadFloat();
            destructionProgress = reader.ReadFloat();
        }

        yield return LoadLevel(version < 2 ? 1 : reader.ReadInt());

        if (version >= 3)
        {
            GameLevel.Current.Load(reader);
        }

        for (int i = 0; i < count; i++)
        {
            int factoryId = version >= 5 ? reader.ReadInt() : 0;
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFactories[factoryId].Get(shapeId);
            instance.Load(reader);
            //shapes.Add(instance);
        }

        for (int i = 0; i < shapes.Count; i++)
        {
            shapes[i].ResolveShapeInstances();
        }
    }

    public void AddShape(Shape shape)
    {
        shape.SaveIndex = shapes.Count;
        shapes.Add(shape);
    }

    public Shape GetShape(int index)
    {
        return shapes[index];
    }

    public void Kill(Shape shape)
    {
        if (inGameUpdateLoop) //多当前正在处理循环,那么先添加至list，后一帧在执行
            killList.Add(shape);
        else
            KillImmediately(shape);//立即处理
    }

    void KillImmediately(Shape shape)
    {
        int index = shape.SaveIndex;
        //回收obj
        shape.Recycle();
        if (index < dyingShapeCount && index < --dyingShapeCount)
        {
            shapes[dyingShapeCount].SaveIndex = index;
            shapes[index] = shapes[dyingShapeCount];
            index = dyingShapeCount;
        }

        int lastIndex = shapes.Count - 1;

        if (index < lastIndex)
        {

            shapes[lastIndex].SaveIndex = index;
            shapes[index] = shapes[lastIndex];
        }

        shapes.RemoveAt(lastIndex);
    }

    
    void MarkAsDyingImmediately(Shape shape)
    {
        //index与dyingShapeCount发生互换，互换之后shapes[index].SaveIndex = index；shapes[dyingShapeCount].SaveIndex = dyingShapeCount;
        //最后相当于排序，前面的是死亡的，后面的是正常的
        int index = shape.SaveIndex;
        if (index < dyingShapeCount)
            return;
        shapes[dyingShapeCount].SaveIndex = index;
        shapes[index] = shapes[dyingShapeCount];
        shape.SaveIndex = dyingShapeCount;
        shapes[dyingShapeCount++] = shape;
    }

    //下一帧标记
    public void MarkAsDying(Shape shape)
    {
        if (inGameUpdateLoop)
            markAsDyingList.Add(shape);
        else
            MarkAsDyingImmediately(shape);
    }

    public bool IsMarkedAsDying(Shape shape)
    {
        return shape.SaveIndex < dyingShapeCount;
    }

}
