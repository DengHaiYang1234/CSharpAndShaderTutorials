using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//小方块方向
public static class MazeDirections
{
    public const int Count = 4;

    public static MazeDirection RandomValue
    {
        get { return (MazeDirection)Random.Range(0, Count); }
    }

    private static IntVector2[] Vectors =
    {
        new IntVector2(0, 1),
        new IntVector2(1, 0),
        new IntVector2(0, -1),
        new IntVector2(-1, 0)
    };

    private static Quaternion[] rotations =
    {
        Quaternion.identity,
        Quaternion.Euler(0f, 90f, 0f),
        Quaternion.Euler(0f, 180f, 0f),
        Quaternion.Euler(0f, 270f, 0f)
    };

    private static MazeDirection[] opposites =
    {
        MazeDirection.South,
        MazeDirection.West,
        MazeDirection.North,
        MazeDirection.East,
    };

    public static Quaternion ToRotation(this MazeDirection direction)
    {
        return rotations[(int)direction];
    }

    //给定方向
    public static IntVector2 ToIntVector2(this MazeDirection direction)
    {
        return Vectors[(int)direction];
    }

    //对面方向
    public static MazeDirection GetOpposite(this MazeDirection direction)
    {
        return opposites[(int)direction];
    }

    //顺时针方向
    public static MazeDirection GetNextClockwise(this MazeDirection direction)
    {
        return (MazeDirection)(((int)direction + 1) % Count);
    }
    //逆时针方向
    public static MazeDirection GetNextCounterclockwise(this MazeDirection direction)
    {
        return (MazeDirection)(((int)direction + Count - 1) % Count);
    }

}


