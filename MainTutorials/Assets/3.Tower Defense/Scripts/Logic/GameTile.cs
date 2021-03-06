﻿using System.Collections;
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


    //累计距离与朝向问题
    GameTile GrowPathTo(GameTile neighbor, Direction direction)
    {
        Debug.Assert(HashPath, "No path!");
        if (neighbor == null || neighbor.HashPath)
            return null;
        //累计距离
        neighbor.distance = distance + 1;
        //举例：下一步的行走的方向
        neighbor.nextOnPath = this;
        //移动至的旋转点
        neighbor.ExitPoint = neighbor.transform.localPosition + direction.GetHalfVector();
        //确定方向
        neighbor.PathDirection = direction;
        //如果是墙，就不添加此路径（相当于绕过去）
        return neighbor.Content.BlocksPath ? null : neighbor;
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
        ExitPoint = transform.localPosition;
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
    public GameTile GrowPathLeft() { return GrowPathTo(left, Direction.Right); }
    //public GameTile GrowPathRight() => GrowPathTo(right);
    public GameTile GrowPathRight() { return GrowPathTo(right, Direction.Left); }
    //public GameTile GrowPathUp() => GrowPathTo(up);
    public GameTile GrowPathUp() { return GrowPathTo(up, Direction.Bottom); }
    //public GameTile GrowPathBottom() => GrowPathTo(bottom);
    public GameTile GrowPathBottom() { return GrowPathTo(bottom, Direction.Up); }



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

    public GameTile NextTileOnPath
    {
        get
        {
            return nextOnPath;
        }
    }


    public GameTileContent Content
    {
        get
        {
            return content;
        }
        set
        {
            Debug.Assert(value != null, "Null assigned to content");
            if (content != null)
            {
                content.Recycle();
            }
            content = value;
            content.transform.localPosition = transform.localPosition;
        }
    }

    //走斜线
    public Vector3 ExitPoint { get; private set; }

    public Direction PathDirection { get; private set; }

    #endregion
}
