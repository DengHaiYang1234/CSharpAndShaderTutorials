using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DyingShapeBehavior : ShapeBehavior
{
    Vector3 originalScale;
	float duration,dyingAge;

	public void Initialize(Shape shape,float duration)
	{
		originalScale = shape.transform.localScale;
		this.duration = duration;
		dyingAge = shape.Age;
		shape.MarkAsDying();
	}

    public override ShapeBehaviorType BehaviorType
    {
        get
		{
			return ShapeBehaviorType.Dying;
		}
    }

    public override void Recycle()
    {
        ShapeBehaviorPool<DyingShapeBehavior>.Reclaim(this);
    }

    public override bool GameUpdate(Shape shape)
    {
		float dyingDuration = shape.Age - dyingAge;
		if(dyingDuration < duration)
		{
			float s = 1 - dyingDuration / duration;
			s = (3f - 2f * s) * s *s;
			shape.transform.localScale =s * originalScale;
			return true;
		}

		shape.Die();
        return false;
    }
	
    public override void Save(GameDataWriter write)
    {
        write.Write(originalScale);
		write.Write(duration);
		write.Write(dyingAge);
    }

    public override void Load(GameDataReader reader)
    {
        originalScale = reader.ReadVector();
		duration = reader.ReadFloat();
		dyingAge = reader.ReadFloat();
    }

}
