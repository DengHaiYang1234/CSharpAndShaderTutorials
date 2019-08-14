using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameLevel : PersistableObject
{
    public static GameLevel Current { get; private set; }
    [UnityEngine.Serialization.FormerlySerializedAs("persistentObjects")]
    [SerializeField] private GameLevelObject[] levelObjects;

    [SerializeField] private SpawnZone spawnZone;

    [SerializeField] int populationLimit;

    public int PopulationLimit
    {
        get
        {
            return populationLimit;
        }
    }

    public void GameUpdate()
    {
        for (int i = 0; i < levelObjects.Length; i++)
        {
            levelObjects[i].GameUpdate();
        }
    }

    private void OnEnable()
    {
        Current = this;
        if (levelObjects == null)
            levelObjects = new GameLevelObject[0];
    }

    public void SpawnShape()
    {
        spawnZone.SpawnShapes();
    }

    public override void Load(GameDataReader reader)
    {
        int savedCount = reader.ReadInt();
        for (int i = 0; i < levelObjects.Length; i++)
            levelObjects[i].Load(reader);
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(levelObjects.Length);
        for (int i = 0; i < levelObjects.Length; i++)
            levelObjects[i].Save(writer);
    }

    

    //private void Start()
    //{
    //    Game.Instance.spawnZoneOfLevel = spawnZone;
    //}
}
