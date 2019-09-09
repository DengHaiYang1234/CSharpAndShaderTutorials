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


    public class State
    {
        EnemySpawnSequence sequence;

        public State(EnemySpawnSequence sequence)
        {
            this.sequence = sequence;
        }
    }

}
