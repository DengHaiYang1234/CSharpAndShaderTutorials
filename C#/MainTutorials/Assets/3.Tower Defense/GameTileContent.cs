using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTileContent : MonoBehaviour 
{
	[SerializeField]
	GameTileContentType type;
	
	//public GameTileContentType Type => type;
	public GameTileContentType Type
	{
		get
		{
			return type;
		}
	}

}
