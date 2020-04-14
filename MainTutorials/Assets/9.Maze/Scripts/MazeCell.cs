using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//单个方块
public class MazeCell : MonoBehaviour
{
    public IntVector2 coordinates;

    //记录当前cell的room
    public MazeRoom room;

    private MazeCellEdge[] edges = new MazeCellEdge[MazeDirections.Count];

    private int initializedEdgeCount = 0;

    public void Initialize(MazeRoom room)
    {
        room.Add(this);
        //设置地板材质
        transform.GetChild(0).GetComponent<Renderer>().material = room.settings.floorMaterial;
    }

    //四个面是否已经完全填满
    public bool IsFullyInitialized
    {
        get { return initializedEdgeCount == MazeDirections.Count; }
    }

    public MazeCellEdge GetEdge(MazeDirection direction)
    {
        return edges[(int)direction];
    }

    //记录已经备份的方向
    public void SetEdge(MazeDirection direction, MazeCellEdge edge)
    {
        edges[(int)direction] = edge;
        initializedEdgeCount += 1;
    }

    //随机一个未被初始化的方向
    public MazeDirection RandomUninitializedDirection
    {
        get
        {
            int skips = Random.Range(0, MazeDirections.Count - initializedEdgeCount);
            for (int i = 0; i < MazeDirections.Count; i++)
            {
                if (edges[i] == null)
                {
                    if (skips == 0)
                    {
                        return (MazeDirection)i;
                    }
                    skips -= 1;
                }
            }
            throw new System.InvalidOperationException("MazeCell has no uninitialized directions left.");
        }
    }

}
