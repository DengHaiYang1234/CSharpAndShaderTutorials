using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteShapeBehavior : ShapeBehavior
{
    //private Shape focalShape;
    private float frequency;
    private Vector3 cosOffset, sinOffset;

    ShapeInstance focalShape;

    Vector3 previousPosition;

    /// <summary>
    /// Satellite初始化
    /// </summary>
    /// <param name="shape"> 当前Obj </param>
    /// <param name="focalShape"> 目标obj </param>
    /// <param name="radius"> 旋转半径 </param>
    /// <param name="frequency"> 频率 </param>
    public void Initialize(Shape shape, Shape focalShape, float radius, float frequency)
    {
        this.focalShape = focalShape;
        this.frequency = frequency;

        //确定旋转轨道轴
        Vector3 orbitAxis = Random.onUnitSphere;

        do
        {
            
            cosOffset = Vector3.Cross(orbitAxis, Random.onUnitSphere).normalized;
        }
        while (cosOffset.sqrMagnitude < 0.1f);
        sinOffset = Vector3.Cross(cosOffset, orbitAxis);
        cosOffset *= radius;
        sinOffset *= radius;

        shape.AddBehavior<RotationShapeBehavior>().AngularVelocity =
            -360f * frequency *
            shape.transform.InverseTransformDirection(orbitAxis); //将世界空间的方向转换为局部空间

        GameUpdate(shape);
        previousPosition = shape.transform.localPosition;
    }

    public override ShapeBehaviorType BehaviorType
    {
        get { return ShapeBehaviorType.Satellite; }
    }

    public override void Recycle()
    {
        ShapeBehaviorPool<SatelliteShapeBehavior>.Reclaim(this);
    }

    public override bool GameUpdate(Shape shape)
    {
        if (focalShape.IsValid)
        {
            float t = 2f * Mathf.PI * frequency * shape.Age;
            previousPosition = shape.transform.localPosition;
            shape.transform.localPosition = focalShape.Shape.transform.localPosition + cosOffset * Mathf.Cos(t) +
                                            sinOffset * Mathf.Sin(t);
            return true;
        }

        shape.AddBehavior<MovementShapeBehavior>().Velocity = (shape.transform.localPosition - previousPosition) / Time.deltaTime;
        return false;
    }

    public override void Save(GameDataWriter write)
    {
        write.Write(focalShape);
        write.Write(frequency);
        write.Write(cosOffset);
        write.Write(sinOffset);
        write.Write(previousPosition);
    }

    public override void Load(GameDataReader reader)
    {
        focalShape = reader.ReadShapeInstance();
        frequency = reader.ReadFloat();
        cosOffset = reader.ReadVector();
        sinOffset = reader.ReadVector();
        previousPosition = reader.ReadVector();
    }

    public override void ResolveShapeInstances()
    {
        focalShape.Resolve();
    }
}
