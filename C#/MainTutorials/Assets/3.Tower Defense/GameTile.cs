using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    [SerializeField]
    Transform arrow;

    //箭头朝向
    GameTile up, right, bottom, left, nextOnPath;

    GameTileContent content;

    int distance;

    static Quaternion
        upRotation = Quaternion.Euler(90f, 0f, 0f),
        rightRotation = Quaternion.Euler(90f, 90f, 0f),
        bottomRotation = Quaternion.Euler(90f, 180f, 0f),
        leftRotation = Quaternion.Euler(90f, 270, 0f);


    public GameTileContent Content
    {
        get
        {
            return content;
        }
        set
        {
            Debug.Assert(value != null,"Null assigned to content");
            if(content != null)
            {
                content.Recycle();
            }
            content = value;
            content.transform.localPosition = transform.localPosition;
        }
    }

    //累计距离与朝向问题
    GameTile GrowPathTo(GameTile neighbor)
    {
        Debug.Assert(HashPath, "No path!");
        if (neighbor == null || neighbor.HashPath)
            return null;
        //累计距离
        neighbor.distance = distance + 1;
        //举例：该tile的右边的nextOnPath就是该tile,也就是箭头应该指的方向
        neighbor.nextOnPath = this;
        return neighbor.Content.Type != GameTileContentType.Wall ? neighbor : null;
    }


    #region Static
    //构建左右或上下关系，采用的是链表的方式
    public static void MakeLeftRightNeighbors(GameTile right, GameTile left)
    {
        Debug.Assert(left.right == null && right.left == null, "Redefined neighbors!");
        left.right = right;
        right.left = left;
    }

    public static void MakeUpBottomNeightbors(GameTile up, GameTile bottom)
    {
        Debug.Assert(up.bottom == null && bottom.up == null, "Redefined neighbors!");
        up.bottom = bottom;
        bottom.up = up;
    }
    #endregion


    #region Public
    //确定正确的方向
    public bool IsAlternative { get; set; }
    //reset
    public void ClearPath()
    {
        distance = int.MaxValue;
        nextOnPath = null;
    }

    public void BecomeDestination()
    {
        distance = 0;
        nextOnPath = null;
    }

    public bool HashPath
    {
        get
        {
            if (distance != int.MaxValue)
                return true;

            return false;
        }
    }

    public void HidePath()
    {
        arrow.gameObject.SetActive(false);
    }



    //public GameTile GrowPathLeft() => GrowPathTo(left);
    public GameTile GrowPathLeft() { return GrowPathTo(left); }
    //public GameTile GrowPathRight() => GrowPathTo(right);
    public GameTile GrowPathRight() { return GrowPathTo(right); }
    //public GameTile GrowPathUp() => GrowPathTo(up);
    public GameTile GrowPathUp() { return GrowPathTo(up); }
    //public GameTile GrowPathBottom() => GrowPathTo(bottom);
    public GameTile GrowPathBottom() { return GrowPathTo(bottom); }


    //变换朝向
    public void ShowPath()
    {
        if (distance == 0)
        {
            arrow.gameObject.SetActive(false);
            return;
        }

        arrow.gameObject.SetActive(true);
        arrow.localRotation =
            nextOnPath == up ? upRotation :
            nextOnPath == left ? leftRotation :
            nextOnPath == bottom ? bottomRotation :
            rightRotation;
    }

    #endregion
}
