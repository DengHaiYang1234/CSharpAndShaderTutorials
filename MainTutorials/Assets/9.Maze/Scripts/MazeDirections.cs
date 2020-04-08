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

    private static MazeDirection[] opposites =
    {
        MazeDirection.South,
        MazeDirection.West,
        MazeDirection.North,
        MazeDirection.East,
    };

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

}


