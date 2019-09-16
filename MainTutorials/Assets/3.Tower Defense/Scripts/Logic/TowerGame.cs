using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameBehaviorCollection
{
    List<GameBehavior> behaviors = new List<GameBehavior>();

    public void Add(GameBehavior behavior)
    {
        behaviors.Add(behavior);
    }

    public void GameUpdate()
    {
        for (int i = 0; i < behaviors.Count; i++)
        {
            if (!behaviors[i].GameUpdate())
            {
                int lastIndex = behaviors.Count - 1;
                behaviors[i] = behaviors[lastIndex];
                behaviors.RemoveAt(lastIndex);
                i -= 1;
            }
        }
    }

    public bool IsEmpty
    {
        get
        {
            return behaviors.Count == 0;
        }
    }

    public void Clear()
    {
        for (int i = 0; i < behaviors.Count; i++)
        {
            behaviors[i].Recycle();
        }
        behaviors.Clear();
    }
}



public class TowerGame : MonoBehaviour
{
    [SerializeField]
    GameScenario scenario;

    [SerializeField, Range(0, 100)]
    int startingPlayerHealth = 10;

    [SerializeField, Range(1f, 10f)]
    float playSpeed = 1f;

    GameScenario.State activeScenario;

    [SerializeField]
    Vector2 boardSize = new Vector2(11, 11);

    [SerializeField]
    GameBoard board;

    [SerializeField]
    GameTileContentFactory tileContentFactory;

    // [SerializeField]
    // EnemyFactory enemyFactory;

    [SerializeField]
    WarFactory warFactory;

    // [SerializeField, Range(0.1f, 10f)]
    // float spawnSpeed = 1f;

    // float spawnProgress;



    GameBehaviorCollection enemies = new GameBehaviorCollection();

    GameBehaviorCollection noEnemies = new GameBehaviorCollection();

    TowerType selectedTowerType;

    int palyerHealth;

    const float pausedTimeScale = 0f;

    static TowerGame instance;

    public static Shell SpawnShell()
    {
        Shell shell = instance.warFactory.Shell;
        instance.noEnemies.Add(shell);
        return shell;
    }

    Ray TouchRay
    {
        get
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }

    void Awake()
    {
        palyerHealth = startingPlayerHealth;
        board.Initialize(boardSize, tileContentFactory);
        board.ShowGrid = true;
        activeScenario = scenario.Begin();
    }

    void OnEnable()
    {
        instance = this;
    }

    void OnValidate()
    {
        if (boardSize.x < 2)
            boardSize.x = 2;

        if (boardSize.y < 2)
            boardSize.y = 2;
    }

    void BeginNewGame()
    {
        palyerHealth = startingPlayerHealth;
        enemies.Clear();
        noEnemies.Clear();
        board.Clear();
        activeScenario = scenario.Begin();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            HandleAlternativeTouch();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            board.ShowPaths = !board.ShowPaths;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            board.ShowGrid = !board.ShowGrid;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = Time.timeScale > pausedTimeScale ? pausedTimeScale : playSpeed;
        }
        else if (Time.deltaTime > pausedTimeScale)
        {
            Time.timeScale = playSpeed;
        }

        if (Input.GetKeyDown(KeyCode.B))
            BeginNewGame();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedTowerType = TowerType.Laser;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedTowerType = TowerType.Mortar;
        }

        // spawnProgress += spawnSpeed * Time.deltaTime;
        // while (spawnProgress >= 1f)
        // {
        //     spawnProgress -= 1f;
        //     SpawnEnemy();
        // }

        if (palyerHealth <= 0 && startingPlayerHealth > 0)
        {
            Debug.Log("Defate!");
            BeginNewGame();
        }

        if (!activeScenario.Progress() && enemies.IsEmpty)
        {
            Debug.Log("Victory!");
            BeginNewGame();
            activeScenario.Progress();
        }

        activeScenario.Progress();

        enemies.GameUpdate();

        board.GameUpdate();

        noEnemies.GameUpdate();
    }

    void HandleTouch()
    {
        GameTile tile = board.GetTile(TouchRay);
        if (tile != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                board.ToggleTower(tile, selectedTowerType);
            }
            else
                board.ToggleWall(tile);
        }
    }

    void HandleAlternativeTouch()
    {
        GameTile tile = board.GetTile(TouchRay);
        if (tile != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                board.ToggleDestination(tile);
            }
            else
                board.ToggleSpawnPoint(tile);
        }
    }

    public static void SpawnEnemy(EnemyFactory factory, EnemyType type)
    {
        GameTile spawnPoint = instance.board.GetSpawnPoint(Random.Range(0, instance.board.SpawnPointCount));
        Enemy enemy = factory.Get((EnemyType)(Random.Range(0, 3)));
        enemy.SpawnOn(spawnPoint);
        instance.enemies.Add(enemy);
    }

    public static Explosion SpawnExplosion()
    {
        Explosion explosion = instance.warFactory.Explosion;
        instance.noEnemies.Add(explosion);
        return explosion;
    }

    public static void EnemyReachedDestination()
    {
        instance.palyerHealth -= 1;
    }

}
