using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//如果物体上已经有了这个组件，再次添加就会弹窗提示.并且添加的操作无效。
[DisallowMultipleComponent]
public class PersistableObject : MonoBehaviour
{
    public virtual void Save(GameDataWriter writer)
    {
        writer.Write(transform.localPosition);
        writer.Write(transform.localRotation);
        writer.Write(transform.localScale);
    }
	
    public virtual void Load(GameDataReader reader)
    {
        transform.localPosition = reader.ReadVector();
        transform.localRotation = reader.ReadQuaternion();
        transform.localScale = reader.ReadVector();
    }

}
