using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameScenario", menuName = "Tower Defense/GameScenario", order = 2)]
public class GameScenario : ScriptableObject 
{
	[SerializeField]
	EnemyWave[] waves = {};
}
