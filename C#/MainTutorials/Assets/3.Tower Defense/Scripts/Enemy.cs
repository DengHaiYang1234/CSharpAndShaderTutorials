using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    Transform model;

    EnemyFactory originFactory;

    GameTile tileFrom, tileTo;

    Vector3 positionFrom, positionTo;

    float progress, progressFactor; //progressFactor,越大表示动作越快

    private Direction direction;
    private DirectionChange directionChange;
    private float directionAngleFrom, directionAngleTo;
    float pathOffest;
    float speed;

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

    public void Initialize(float scale,float pathOffest,float speed)
    {
        model.localScale = new Vector3(scale,scale,scale);
        this.speed = speed;
        this.pathOffest = pathOffest;
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
        progress += Time.deltaTime * progressFactor;

        while (progress >= 1f)
        {
            if (tileTo == null)
            {
                OriginFactory.Reclaim(this);
                return false;
            }

            progress = (progress - 1f) / progressFactor;
            PrepareNextState();
            progress *= progressFactor;
        }

        if (directionChange == DirectionChange.None)
        {
            //不转向，一直往前走
            transform.localPosition = Vector3.LerpUnclamped(positionFrom, positionTo, progress);
        }
        else
        {
            //转向
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
        model.localPosition = new Vector3(pathOffest,0f);
        //获取当前的旋转欧拉角
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f * speed;
    }

    void PrepareNextState()
    {
        tileFrom = tileTo;
        tileTo = tileTo.NextTileOnPath;
        //将上一个位置终点当做此次位置起点。依次往复，直到达到终点
        positionFrom = positionTo;
        if(tileTo == null)
        {
            PrepareOutro();
            return;
        }
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

    //结尾
    void PrepareOutro()
    {
        positionTo = tileFrom.transform.localPosition;
        directionChange = DirectionChange.None;
        directionAngleTo = direction.GetAngle();
        model.localPosition = new Vector3(pathOffest,0f);
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f * speed;
    }

    void PrepareForward()
    {
        //获取当前旋转
        transform.localRotation = direction.GetRotation();
        //变换方向的角度
        directionAngleTo = direction.GetAngle();
        //以自身旋转
        model.localPosition = new Vector3(pathOffest,0f);
        progressFactor = speed;
    }

    void PrepareTurnRight()
    {
        directionAngleTo = directionAngleFrom + 90f;
        //以0.5为半径旋转
        model.localPosition = new Vector3(pathOffest - 0.5f,0);
        //向朝向方向移动0.5个距离
        transform.localPosition = positionFrom + direction.GetHalfVector();
        //圆周或圆等于其半径的2π倍。右转或左转只占四分之一，半径是1/2，所以是1/2π×1/2。
        progressFactor = speed / (Mathf.PI * 0.5f * (0.5f - pathOffest));
    }
    
    void PrepareTurnLeft()
    {
        directionAngleTo = directionAngleFrom - 90f;
        //以0.5为半径旋转
        model.localPosition = new Vector3(pathOffest + 0.5f, 0f);
        //向朝向方向移动0.5个距离
        transform.localPosition = positionFrom + direction.GetHalfVector();
        //圆周或圆等于其半径的2π倍。右转或左转只占四分之一，半径是1/2，所以是1/2π×1/2。
        progressFactor = speed / (Mathf.PI * 0.5f * (0.5f + pathOffest));
    }
    

    void PrepareTurnAround()
    {
        directionAngleTo = directionAngleFrom + (pathOffest < 0f ? 180f : -180f);
        //以自身旋转
        model.localPosition = new Vector3(pathOffest,0f);
        //向朝向方向移动0.5个距离
        transform.localPosition = positionFrom;
        progressFactor = speed / (Mathf.PI * Mathf.Max(Mathf.Abs(pathOffest),0.2f));
    }

}
