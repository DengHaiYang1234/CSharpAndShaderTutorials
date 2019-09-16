using UnityEngine;

[System.Serializable]
public class EnemySpawnSequence
{
    [SerializeField]
    EnemyFactory factory;

    [SerializeField]
    EnemyType type = EnemyType.Medium;

    [SerializeField, Range(1, 100)]
    int amount = 1;

    [SerializeField, Range(0.1f, 10f)]
    float cooldown = 1f;

    public State Begin()
    {
        return new State(this);
    }

    [System.Serializable]
    public struct State
    {
        int count;
        //冷却时间
        float cooldown;
        EnemySpawnSequence sequence;

        public State(EnemySpawnSequence sequence)
        {
            this.sequence = sequence;
            count = 0;
            cooldown = sequence.cooldown;
        }

        //转CD
        public float Progress(float deltaTime)
        {
            cooldown += deltaTime;
            while (cooldown >= sequence.cooldown)
            {
                cooldown -= sequence.cooldown;
                if (count >= sequence.amount)
                    return cooldown;
                count += 1;
                TowerGame.SpawnEnemy(sequence.factory, sequence.type);
            }
            return -1f;
        }
    }
}


