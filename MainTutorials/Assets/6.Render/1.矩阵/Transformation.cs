using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Transformation : MonoBehaviour 
{
	public abstract Vector3 Apply(Vector3 point);

}
