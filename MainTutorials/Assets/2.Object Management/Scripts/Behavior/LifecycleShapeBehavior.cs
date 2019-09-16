using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifecycleShapeBehavior : ShapeBehavior
{

	Vector3 originalScale;
	float adultDuration,dyingDuration,dyingAge;

    /// <summary>
    /// Lifecycle初始化
    /// </summary>
    /// <param name="shape"> obj </param>
    /// <param name="growingDuration"> 生成时间 </param>
    /// <param name="adultDuration"> 持续时间 </param>
    /// <param name="dyingDuration"> 死亡时间 </param>
	public void Initialize(Shape shape,float growingDuration,float adultDuration,float dyingDuration)
	{
		this.adultDuration = adultDuration;
		this.dyingDuration = dyingDuration;
        //存活时间总时长
		dyingAge = growingDuration + adultDuration;

		if(growingDuration > 0f)
		{
            //开始生成
			shape.AddBehavior<GrowingShapeBehavior>().Initialize(shape,growingDuration);
		}
	}

    public override ShapeBehaviorType BehaviorType
    {
        get
		{
			return ShapeBehaviorType.Lifecycle;
		}
    }

    public override void Recycle()
    {
        ShapeBehaviorPool<LifecycleShapeBehavior>.Reclaim(this);
    }

    public override bool GameUpdate(Shape shape)
    {
        //若当前的已达到规定的存活总时长
		if(shape.Age >= dyingAge)
		{
            //死亡时间为0是，直接死亡
			if(dyingDuration <= 0f)
			{
				shape.Die();
				return true;
			}
			
			if(!shape.IsMarkAsDying)
			{
				shape.AddBehavior<DyingShapeBehavior>().Initialize(shape,dyingDuration + dyingAge - shape.Age);
			}	

			shape.AddBehavior<DyingShapeBehavior>().Initialize(shape,dyingDuration + dyingAge - shape.Age);
			return false;
		}

		return true;
    }
	
    public override void Save(GameDataWriter write)
    {
        write.Write(adultDuration);
		write.Write(dyingDuration);
		write.Write(dyingAge);
    }

    public override void Load(GameDataReader reader)
    {
        adultDuration = reader.ReadFloat();
		dyingDuration = reader.ReadFloat();
		dyingAge = reader.ReadFloat();
    }

}
