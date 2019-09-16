using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWave", menuName = "Tower Defense/EnemyWave", order = 2)]
[System.Serializable]
public class EnemyWave : ScriptableObject
{
    [SerializeField]
    EnemySpawnSequence[] spawnSequences = { new EnemySpawnSequence() };

    public State Begin()
    {
        return new State(this);
    }

    public struct State
    {
        EnemyWave wave;
        int index;
        EnemySpawnSequence.State sequene;

        public State(EnemyWave wave)
        {
            this.wave = wave;
            index = 0;
            Debug.Assert(wave.spawnSequences.Length > 0, "Empty wave!");
            sequene = wave.spawnSequences[0].Begin();
        }

        
        public float Progress(float deltaTime)
        {
            deltaTime = sequene.Progress(deltaTime);
            while (deltaTime >= 0f)
            {
                if (++index >= wave.spawnSequences.Length)
                {
                    return deltaTime;
                }

                sequene = wave.spawnSequences[index].Begin();
                deltaTime = sequene.Progress(deltaTime);
            }
            return -1f;
        }
    }
}
