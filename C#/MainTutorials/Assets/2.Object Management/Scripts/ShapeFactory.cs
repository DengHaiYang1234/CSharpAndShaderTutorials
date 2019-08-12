using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//资源类，根据已设置好的资源来创建实例
[CreateAssetMenu]
public class ShapeFactory : ScriptableObject
{


    [SerializeField]
    Shape[] prefabs;

    [SerializeField]
    Material[] materials;

    [SerializeField]
    private bool recycle;
    [SerializeField]
    int factoryId = int.MinValue;

    //对象池
    private List<Shape>[] pools;

    //场景池
    private Scene poolScene;

    public int FactoryId
    {
        get
        {
            return factoryId;
        }
        set
        {
            if(factoryId == int.MinValue && value != int.MinValue)
            {
                factoryId = value;
            }
            else
                Debug.LogError("Not allowed to change factoryId.");
        }
    }

    public Shape Get(int shapeId = 0, int materialId = 0)
    {
        Shape instance;
        if (recycle)
        {
            if (pools == null)
                CreatPools();

            List<Shape> pool = pools[shapeId];
            int lastIndex = pool.Count - 1;
            if (lastIndex >= 0)
            {
                instance = pool[lastIndex];
                instance.gameObject.SetActive(true);
                pool.RemoveAt(lastIndex);
            }
            else
            {
                instance = Instantiate(prefabs[shapeId]);
                instance.OriginFactory = this;
                instance.ShapeId = shapeId;
                //移动至指定场景中
                SceneManager.MoveGameObjectToScene(instance.gameObject, poolScene);
            }
        }
        else
        {
            instance = Instantiate(prefabs[shapeId]);
            instance.ShapeId = shapeId;
        }

        instance.SetMaterial(materials[materialId], materialId);
        return instance;
    }

    public Shape GetRandom()
    {
        return Get(Random.Range(0, prefabs.Length),
            Random.Range(0, materials.Length));
    }

    //回收
    public void Reclaim(Shape shapeToRecycle)
    {
        if(shapeToRecycle.OriginFactory != this)
        {
            Debug.LogError("Tried to reclaim shape with wrong factory!");
            return;
        }

        if (recycle)
        {
            if (pools == null)
                CreatPools();
            pools[shapeToRecycle.ShapeId].Add(shapeToRecycle);
            shapeToRecycle.gameObject.SetActive(false);
        }
        else
        {
            Destroy(shapeToRecycle.gameObject);
        }
    }

    //初始化对象池
    void CreatPools()
    {
        pools = new List<Shape>[prefabs.Length];
        //自动编译之后，序列化数据会丢失
        if (Application.isEditor)
        {
            poolScene = SceneManager.GetSceneByName(name);
            if (poolScene.isLoaded)
            {
                GameObject[] rootObjects = poolScene.GetRootGameObjects();
                for (int i = 0; i < rootObjects.Length; i++)
                {
                    Shape pooledShape = rootObjects[i].GetComponent<Shape>();
                    if (!pooledShape.gameObject.activeSelf)
                    {
                        pools[pooledShape.ShapeId].Add(pooledShape);
                    }
                }
                return;
            }
        }

        poolScene = SceneManager.CreateScene(name);

        for (int i = 0; i < pools.Length; i++)
        {
            pools[i] = new List<Shape>();
        }
    }



}
