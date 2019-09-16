
using UnityEngine;

[System.Serializable]
public struct TowerFloatRange  
{
	[SerializeField]
	float min,max;

	public float Min{
		get
		{
			return min;
		}
	}

	public float Max
	{
		get
		{
			return max;
		}
	}


	public float RandomValueInRange{
		get
		{
			return Random.Range(min,max);
		}
	}

	public TowerFloatRange(float value)
	{
		min = max = value;
	}

	public TowerFloatRange(float min,float max)
	{
		this.min = min;
		this.max = max;
	}

}
