using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//生成各式不同形状
public abstract class SpawnZone : MonoBehaviour
{
    public abstract Vector3 SpawnPoint { get; }
}
