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
}



public class TowerGame : MonoBehaviour
{
    [SerializeField]
    Vector2 boardSize = new Vector2(11, 11);

    [SerializeField]
    GameBoard board;

    [SerializeField]
    GameTileContentFactory tileContentFactory;

    [SerializeField]
    EnemyFactory enemyFactory;

    [SerializeField]
    WarFactory warFactory;

    [SerializeField, Range(0.1f, 10f)]
    float spawnSpeed = 1f;
    
    float spawnProgress;

    GameBehaviorCollection enemies = new GameBehaviorCollection();

    GameBehaviorCollection noEnemies = new GameBehaviorCollection();

    TowerType selectedTowerType;

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
        board.Initialize(boardSize, tileContentFactory);
        board.ShowGrid = true;
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedTowerType = TowerType.Laser;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedTowerType = TowerType.Mortar;
        }

        spawnProgress += spawnSpeed * Time.deltaTime;
        while (spawnProgress >= 1f)
        {
            spawnProgress -= 1f;
            SpawnEnemy();
        }
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

    void SpawnEnemy()
    {
        GameTile spawnPoint = board.GetSpawnPoint(Random.Range(0, board.SpawnPointCount));
        Enemy enemy = enemyFactory.Get();
        enemy.SpawnOn(spawnPoint);
        enemies.Add(enemy);
    }

    public static Explosion SpawnExplosion()
    {
        Explosion explosion = instance.warFactory.Explosion;
        instance.noEnemies.Add(explosion);
        return explosion;
    }

}
