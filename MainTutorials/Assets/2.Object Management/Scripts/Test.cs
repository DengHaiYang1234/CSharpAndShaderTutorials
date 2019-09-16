using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	public Transform root;
	// Use this for initialization
	
	void Start () 
	{
		ChangeColor();
	}

	void ChangeColor()
	{
		Transform[] ts = root.GetComponentsInChildren<Transform>();
		GameObject[] objects = new GameObject[ts.Length];
		for(int i = 0; i < ts.Length;i++)
		{
			objects[i] = ts[i].gameObject;
		}

		MaterialPropertyBlock props = new MaterialPropertyBlock();
		MeshRenderer render;

		foreach(var obj in objects)
		{
			float r = Random.Range(0.0f,1.0f);
			float g = Random.Range(0.0f,1.0f);
			float b = Random.Range(0.0f,1.0f);
			props.SetColor("_Color",new Color(r,g,b));

			render = obj.GetComponent<MeshRenderer>();
			if(render == null)
				continue;

			render.SetPropertyBlock(props);
		}
	}
	

}
