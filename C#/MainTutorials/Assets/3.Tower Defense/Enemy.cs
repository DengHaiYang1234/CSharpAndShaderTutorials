using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    EnemyFactory originFactory;

    GameTile tileFrom, tileTo;

    Vector3 positionFrom, positionTo;

    float progress;

    private Direction direction;
    private DirectionChange directionChange;
    private float directionAngleFrom, directionAngleTo;

    public EnemyFactory OriginFactory
    {
        get
        {
            return originFactory;
        }
        set
        {
            Debug.Assert(originFactory == null, "Redefined origin factory");
            originFactory = value;
        }
    }


    public void SpawnOn(GameTile tile)
    {
        Debug.Assert(tile.NextTileOnPath != null, "Nowhere to go!", this);
        //起点
        tileFrom = tile;
        //终点
        tileTo = tile.NextTileOnPath;

        progress = 0f;

        PrepareIntro();
    }

    public bool GameUpdate()
    {
        progress += Time.deltaTime;

        while (progress >= 1f)
        {

            tileFrom = tileTo;
            tileTo = tileTo.NextTileOnPath;
            if (tileTo == null)
            {
                OriginFactory.Reclaim(this);
                return false;
            }

            progress -= 1f;
            PrepareNextState();
        }

        transform.localPosition = Vector3.LerpUnclamped(positionFrom, positionTo, progress);
        if (directionChange != DirectionChange.None)
        {
            float angle = Mathf.LerpUnclamped(directionAngleFrom, directionAngleTo, progress);
            transform.localRotation = Quaternion.Euler(0f, angle, 0f);
        }
        return true;
    }


    void PrepareIntro()
    {
        //起点坐标
        positionFrom = tileFrom.transform.localPosition;
        //终点坐标
        positionTo = tileFrom.ExitPoint;
        //起点方向
        direction = tileFrom.PathDirection;
        //方向变化
        directionChange = DirectionChange.None;
        //获取当前方向的角度及若变换到下一方向的角度
        directionAngleFrom = directionAngleTo = direction.GetAngle();
        //获取当前的旋转欧拉角
        transform.localRotation = direction.GetRotation();
    }


    void PrepareNextState()
    {
        //将上一个位置终点当做此次位置起点。依次往复，直到达到终点
        positionFrom = positionTo;
        positionTo = tileFrom.ExitPoint;
        //将上一方向变换至下一方向,并缓存当前方向变换
        directionChange = direction.GetDirectionChangeTo(tileFrom.PathDirection);
        //当前方向修改至以变换的方向
        direction = tileFrom.PathDirection;
        //记录起点变换角度
        directionAngleFrom = directionAngleTo;
        //根据变换的下一方向，来决定终点变换的角度
        switch (directionChange)
        {
            case DirectionChange.None:
                PrepareForward();
                break;
            case DirectionChange.TurnRight:
                PrepareTurnRight();
                break;
            case DirectionChange.TurnLeft:
                PrepareTurnLeft();
                break;
            default:
                PrepareTurnAround();
                break;
        }
    }

    void PrepareForward()
    {
        //获取当前旋转
        transform.localRotation = direction.GetRotation();
        //变换方向的角度
        directionAngleTo = direction.GetAngle();
    }

    void PrepareTurnRight()
    {
        directionAngleTo = directionAngleFrom + 90f;
    }

    void PrepareTurnLeft()
    {
        directionAngleTo = directionAngleFrom - 90f;
    }


    void PrepareTurnAround()
    {
        directionAngleTo = directionAngleFrom + 90f;
    }

}
