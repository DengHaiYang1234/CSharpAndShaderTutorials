using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//边与对边的信息
public abstract class MazeCellEdge : MonoBehaviour
{
    //当前cell与对面cell
    public MazeCell cell, otherCell;
    //朝向
    public MazeDirection direction;


    public virtual void Initialize(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        this.cell = cell;
        this.otherCell = otherCell;
        this.direction = direction;
        cell.SetEdge(direction, this);
        //放置于原来的cell
        transform.parent = cell.transform;
        transform.localPosition = Vector3.zero;
        //旋转角度
        transform.localRotation = direction.ToRotation();
    }


    public virtual void OnPlayerEntered()
    {
    }

    public virtual void OnPlayerExited()
    {
        
    }



}
