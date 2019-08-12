using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//实例化的obj向圆球聚拢
public class NucleonSpawner : MonoBehaviour
{
    [Header("生成时间间隔")]
	public float timeBetweenSpawns;
    [Header("距离间隔")]
    public float spawnDistance;
    [Header("生成的对象")]
    public Nucleon[] nucleonPrefabs;

	float timeSinceLastSpawn;
	
	void SpawnNucleon()
	{
		Nucleon prefab = nucleonPrefabs[Random.Range(0,nucleonPrefabs.Length)];
		Nucleon spawn = Instantiate<Nucleon>(prefab);
		spawn.transform.localPosition = Random.onUnitSphere * spawnDistance;
	}

	void FixedUpdate()
	{
		timeSinceLastSpawn += Time.deltaTime;
		if(timeSinceLastSpawn >= timeBetweenSpawns)
		{
			timeSinceLastSpawn -= timeBetweenSpawns;
			SpawnNucleon();
		}
	}


}
