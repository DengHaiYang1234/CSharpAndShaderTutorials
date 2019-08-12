using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGame : MonoBehaviour 
{
	[SerializeField]
	Vector2 boardSize = new Vector2(11,11);

	[SerializeField]
	GameBoard board;

	void Awake()
	{
		board.Initialize(boardSize);
	}
	
	void OnValidate()
	{
		if(boardSize.x < 2)
			boardSize.x = 2;

		if(boardSize.y < 2)
			boardSize.y = 2;	
	}

}
