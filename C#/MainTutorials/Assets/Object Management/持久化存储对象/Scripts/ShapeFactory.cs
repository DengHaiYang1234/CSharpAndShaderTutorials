using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//资源类，根据已设置好的资源来创建实例
[CreateAssetMenu]
public class ShapeFactory : ScriptableObject
{
    [SerializeField]
    Shape[] prefabs;
	[SerializeField]
	Material[] materials;
	
    public Shape Get(int shapeId = 0,int materialId = 0)
    {
        Shape instance = Instantiate(prefabs[shapeId]);
		instance.ShapeId = shapeId;
		instance.SetMaterial(materials[materialId],materialId);
		return instance;
    }

    public Shape GetRandom()
    {
		return Get(Random.Range(0,prefabs.Length),
			Random.Range(0,materials.Length));
    }

}
