using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeSpawnZone : SpawnZone
{
    [SerializeField]
    private bool sequential;

    [SerializeField]
    SpawnZone[] spawnZones;

    [SerializeField]
    private bool overrideConfig;

    private int nextSequentialIndex;

    public override Vector3 SpawnPoint
    {
        get
        {
            int index;
            //次序生成
            if (sequential)
            {
                index = nextSequentialIndex++;
                if (nextSequentialIndex >= spawnZones.Length)
                    nextSequentialIndex = 0;
            }
            else
                index = UnityEngine.Random.Range(0, spawnZones.Length);

            return spawnZones[index].SpawnPoint;
        }
    }

    public override void ConfigureSpawn(Shape shape)
    {
        if (overrideConfig)
            base.ConfigureSpawn(shape);
        else
        {
            int index;
            //次序生成
            if (sequential)
            {
                index = nextSequentialIndex++;
                if (nextSequentialIndex >= spawnZones.Length)
                    nextSequentialIndex = 0;
            }
            else
                index = UnityEngine.Random.Range(0, spawnZones.Length);

            spawnZones[index].ConfigureSpawn(shape);
        }
    }

    public override void Load(GameDataReader reader)
    {
        nextSequentialIndex = reader.ReadInt();
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(nextSequentialIndex);
    }
}
