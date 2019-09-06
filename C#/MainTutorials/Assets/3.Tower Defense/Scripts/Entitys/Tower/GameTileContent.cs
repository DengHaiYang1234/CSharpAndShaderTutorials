using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class GameTileContent : MonoBehaviour 
{
	[SerializeField]
	GameTileContentType type;

	GameTileContentFactory originFactory;
	
	//public GameTileContentType Type => type;

	public virtual void GameUpdate()
	{

	}

	public bool BlocksPath
	{
		get
		{
			return Type == GameTileContentType.Wall || Type == GameTileContentType.Tower;
		}
	}
	
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
