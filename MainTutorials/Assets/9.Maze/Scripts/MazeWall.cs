using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//墙
public class MazeWall : MazeCellEdge
{

    public override void Initialize(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        base.Initialize(cell,otherCell,direction);
        transform.GetChild(0).GetComponent<Renderer>().material = cell.room.settings.wallMaterial;
    }
}
