using UnityEngine;

public sealed class MovementShapeBehavior : ShapeBehavior 
{
    //速度
	public Vector3 Velocity{get;set;}

	public override ShapeBehaviorType BehaviorType
	{
		get
		{
			return ShapeBehaviorType.Movement;
		}
	}

	public override void Recycle()
	{
		ShapeBehaviorPool<MovementShapeBehavior>.Reclaim(this);
	}

	public override bool GameUpdate(Shape shape)
	{
		shape.transform.localPosition += Velocity * Time.deltaTime;
		return true;
	}

	public override void Save(GameDataWriter write)
	{
		write.Write(Velocity);
	}

	public override void Load(GameDataReader reader)
	{
		Velocity = reader.ReadVector();
	}

}
