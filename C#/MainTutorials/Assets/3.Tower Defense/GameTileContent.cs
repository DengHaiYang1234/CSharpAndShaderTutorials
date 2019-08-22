using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTileContent : MonoBehaviour 
{
	[SerializeField]
	GameTileContentType type;

	GameTileContentFactory originFactory;
	
	//public GameTileContentType Type => type;
	public GameTileContentType Type
	{
		get
		{
			return type;
		}
	}

	public GameTileContentFactory OriginFactory{
		get
		{
			return originFactory;
		}
		set
		{
			Debug.Assert(originFactory == null,"Redefined origin factory!");
			originFactory = value;
		}
	}

	//回收
	public void Recycle()
	{
		originFactory.Reclaim(this);
	}

}
