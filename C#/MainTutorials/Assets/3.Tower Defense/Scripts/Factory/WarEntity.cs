using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WarEntity : GameBehavior 
{
	WarFactory originFactory;

    public WarFactory OriginFactory
    {
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

    public void Recycle()
    {
        originFactory.Reclaim(this);
    }
}
