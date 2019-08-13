using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifecycleShapeBehavior : ShapeBehavior
{

	Vector3 originalScale;
	float adultDuration,dyingDuration,dyingAge;

	public void Initialize(Shape shape,float growingDuration,float adultDuration,float dyingDuration)
	{
		this.adultDuration = adultDuration;
		this.dyingDuration = dyingDuration;
		dyingAge = growingDuration + adultDuration;

		if(growingDuration > 0f)
		{
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
		if(shape.Age >= dyingAge)
		{
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
