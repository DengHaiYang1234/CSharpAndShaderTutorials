using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeDoor : MazePassage
{
    public Transform hinge;

    //对立的门
    MazeDoor OtherSideOfDoor
    {
        get { return otherCell.GetEdge(direction.GetOpposite()) as MazeDoor; }
    }

    //是两个门的把手朝向方向一致
    public override void Initialize(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        base.Initialize(cell, otherCell, direction);
        if (OtherSideOfDoor != null)
        {
            hinge.localScale = new Vector3(-1f, 1f, 1f);
            Vector3 p = hinge.localPosition;
            p.x = -p.x;
            hinge.localPosition = p;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child != hinge)
                child.GetComponent<Renderer>().material = cell.room.settings.wallMaterial;
        }
    }
}
