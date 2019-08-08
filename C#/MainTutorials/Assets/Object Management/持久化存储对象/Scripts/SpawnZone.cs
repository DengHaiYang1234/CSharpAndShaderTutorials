using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//生成各式不同形状
//注：Random生成的各种形状的点位都是伪随机。如果重新加载就不一定跟上次一样了！！
public abstract class SpawnZone : MonoBehaviour
{
    public abstract Vector3 SpawnPoint { get; }
}
