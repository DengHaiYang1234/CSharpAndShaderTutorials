using UnityEngine;

public class TowerFloatRangeSliderAttribute : PropertyAttribute 
{
	public float Min{get;private set;}

	public float Max{get;private set;}

	public TowerFloatRangeSliderAttribute(float min,float max)
	{
		Min = min;
		Max = max < min ? min : max;
	}
}
