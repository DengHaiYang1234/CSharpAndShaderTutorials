﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel : PersistableObject
{
    public static GameLevel Current { get; private set; }

    [SerializeField] private PersistableObject[] persistentObjects;

    [SerializeField] private SpawnZone spawnZone;

    [SerializeField] int populationLimit;

    public int PopulationLimit
    {
        get
        {
            return populationLimit;
        }
    }
    
    private void OnEnable()
    {
        Current = this;
        if (persistentObjects == null)
            persistentObjects = new PersistableObject[0];
    }

    public void SpawnShape()
    {
        spawnZone.SpawnShapes();
    }

    public override void Load(GameDataReader reader)
    {
        int savedCount = reader.ReadInt();
        for (int i = 0; i < persistentObjects.Length; i++)
            persistentObjects[i].Load(reader);
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(persistentObjects.Length);
        for (int i = 0; i < persistentObjects.Length; i++)
            persistentObjects[i].Save(writer);
    }

    //private void Start()
    //{
    //    Game.Instance.spawnZoneOfLevel = spawnZone;
    //}
}