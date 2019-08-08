using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Game流程控制类
public class Game : PersistableObject
{

    [Header("序列化对象")]
    [SerializeField]
    public ShapeFactory shapeFactory;
    [Header("序列化管理器")]
    public PersistentStorage storage;

    [SerializeField] private Slider creationSpeedSlider;
    [SerializeField] private Slider destructionSpeedSlider;

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

    //每个版本代表不同的意思。比如4就是存储的旋转
    const int saveVersion = 4;

    //创建于销毁的控制条
    private float creationProgress, destructionProgress;

    private int loadedLevelBuildIndex;

    Random.State mainRandomState;

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

        for (int i = 0; i < shapes.Count; i++)
        {
            shapes[i].GameUpdate();
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
        //Transform t = instance.transform;
        ////返回半径为1的球体内的随机点
        ////Game -> GameLevle -> Spawn Zone
        //t.localPosition = GameLevel.Current.SpawnPoint;
        ////随机旋转值
        //t.localRotation = Random.rotation;
        //t.localScale = Vector3.one * Random.Range(0.1f, 1f);

        ////改变颜色HSV
        ////https://blog.csdn.net/zgjllf1011/article/details/79391241
        //instance.SetColor(Random.ColorHSV(
        //    hueMin: 0f, hueMax: 1f,
        //    saturationMin: 0.5f, saturationMax: 1f,
        //    valueMin: 0.25f, valueMax: 1f,
        //    alphaMin: 1f, alphaMax: 1f
        //));
        //instance.AngularVelocity = Random.onUnitSphere* Random.Range(0f,90f);
        //instance.Velocity = Random.onUnitSphere*Random.Range(0f,2f);

        GameLevel.Current.ConfigureSpawn(instance);
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
        creationSpeedSlider.value = CreationSpeed = 0;
        destructionSpeedSlider.value = DestructionSpeed = 0;

        Random.state = mainRandomState;
        //随机种子
        int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledDeltaTime;
        mainRandomState = Random.state;
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
        writer.Write(CreationSpeed);
        writer.Write(creationProgress);
        writer.Write(DestructionSpeed);
        writer.Write(destructionProgress);
        writer.Write(loadedLevelBuildIndex);
        GameLevel.Current.Save(writer);
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
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFactory.Get(shapeId);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }
}
