using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWave", menuName = "Tower Defense/EnemyWave", order = 2)]
[System.Serializable]
public class EnemyWave : ScriptableObject
{
    [SerializeField]
    EnemySpawnSequence[] spawnSequences = {new EnemySpawnSequence()};
}
