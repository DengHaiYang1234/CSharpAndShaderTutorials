
using UnityEngine;

public class GrowingShapeBehavior : ShapeBehavior
{
	Vector3 originalScale;
	float duration,dyingAge;

	public void Initialize(Shape shape,float duration)
	{
		originalScale = shape.transform.localScale;
		this.duration = duration;
        //定格记录开始生成的节点
		dyingAge = shape.Age;
	}

    public override ShapeBehaviorType BehaviorType
    {
        get
		{
			return ShapeBehaviorType.Growing;
		}
    }

    public override void Recycle()
    {
        ShapeBehaviorPool<GrowingShapeBehavior>.Reclaim(this);
    }
	
    public override bool GameUpdate(Shape shape)
    {
        //差值，越来越大
        float dyingDuration = shape.Age - dyingAge;

        //在规定的生成时间内，obj还没有完全Init
		if(dyingDuration < duration)
		{
			float s = 1 - dyingDuration / duration;
			s = (3f - 2f * s) * s *s;
			shape.transform.localScale = s * originalScale;
			return true;
		}
        //???只要在规定的生成时间生成好了，那么就置0
		shape.transform.localScale = Vector3.zero;
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
