using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGame : MonoBehaviour 
{
	[SerializeField]
	Vector2 boardSize = new Vector2(11,11);

	[SerializeField]
	GameBoard board;

	[SerializeField]
	GameTileContentFactory tileContentFactory;

	Ray TouchRay
	{
		get
		{
			return Camera.main.ScreenPointToRay(Input.mousePosition);
		}
	}

	void Awake()
	{
		board.Initialize(boardSize,tileContentFactory);
		board.ShowGrid = true;
	}

	void OnValidate()
	{
		if(boardSize.x < 2)
			boardSize.x = 2;

		if(boardSize.y < 2)
			boardSize.y = 2;	
	}

	void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			HandleTouch();
		}
		else if(Input.GetMouseButtonDown(1))
		{
			HandleAlternativeTouch();
		}

		if(Input.GetKeyDown(KeyCode.V))
		{
			board.ShowPaths = !board.ShowPaths;
		}

		if(Input.GetKeyDown(KeyCode.G))
		{
			board.ShowGrid = !board.ShowGrid;
		}
	}

	void HandleTouch()
	{
		GameTile tile = board.GetTile(TouchRay);
		if(tile != null){
			board.ToggleWall(tile);
		}
	}

	void HandleAlternativeTouch()
	{
		GameTile tile = board.GetTile(TouchRay);
		if(tile != null)
		{
			board.ToggleDestination(tile);
		}
	}

}
