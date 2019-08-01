using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Fractal : MonoBehaviour
{
    [Header("mesh集合")]
    public Mesh[] meshs;
    [Header("材质")]
    public Material material;
    [Header("子节点最大深度")]
    public int maxDepth;
    private int depth = 0;
    [Header("child缩放")]
    public float childScale = 0.5f;
    [Header("child生成概率")]
    public float sqawnProbability;
    [Header("最大旋转速度")]
    public float maxRotationSpeed;
    [Header("最大旋转角度")]
    public float maxTwist;

	private float rotationSpeed;

    private Material[,] materials;

    private static Vector3[] childDirections =
    {
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back,
    };

    private static Quaternion[] childQuaternion =
    {
        Quaternion.identity,
        Quaternion.Euler(0f,0f,-90f),
        Quaternion.Euler(0f,0f,90f),
        Quaternion.Euler(90f,0f,0f),
        Quaternion.Euler(-90f,0f,0f),
    };

    void Start()
    {
		rotationSpeed = Random.Range(-maxRotationSpeed,maxRotationSpeed);
		transform.Rotate(Random.Range(-maxTwist,maxTwist),0f,0f);
        if (materials == null)
            InitializeMaterials();

        gameObject.AddComponent<MeshFilter>().mesh = meshs[Random.Range(0, meshs.Length)];
        gameObject.AddComponent<MeshRenderer>().material = materials[depth, Random.Range(0, 2)];
        if (depth < maxDepth)
        {
            StartCoroutine(CreatChildren());
        }
    }

    private IEnumerator CreatChildren()
    {
        for (int i = 0; i < childDirections.Length; i++)
        {
            if (Random.value < sqawnProbability)
            {
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
                new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, childDirections[i], childQuaternion[i]);
            }
        }
    }


    //优化动态合批   减少drawcall
    private void InitializeMaterials()
    {
        materials = new Material[maxDepth + 1, 2];
        for (int i = 0; i <= maxDepth; i++)
        {
            float t = i / (maxDepth - 1f);
            t *= t;
            //每depth两种颜色
            materials[i, 0] = new Material(material);
            materials[i, 0].color = Color.Lerp(Color.white, Color.yellow, t);
            materials[i, 1] = new Material(material);
            materials[i, 1].color = Color.Lerp(Color.white, Color.cyan, t);
        }
        materials[maxDepth, 0].color = Color.magenta;
        materials[maxDepth, 1].color = Color.red;
    }

    private void Initialize(Fractal parent, Vector3 directions, Quaternion quaternion)
    {
        meshs = parent.meshs;
        materials = parent.materials;
        maxDepth = parent.maxDepth;
		sqawnProbability = parent.sqawnProbability;
		maxRotationSpeed = parent.maxRotationSpeed;
		maxTwist = parent.maxTwist;
        depth = parent.depth + 1;
        childScale = parent.childScale;
        transform.parent = parent.transform;
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = directions * (0.5f + 0.5f * childScale);
        transform.localRotation = quaternion;
    }



    void Update()
    {
		transform.Rotate(0f,rotationSpeed * Time.deltaTime,0f);
    }
}
