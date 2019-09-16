using UnityEngine;

public sealed class OscillationShapeBehavior : ShapeBehavior
{
    public Vector3 Offset{get;set;}

	public float Frequency{get;set;}

	float previousOscillation;

    public override ShapeBehaviorType BehaviorType
    {
        get
        {
            return ShapeBehaviorType.Oscillation;
        }
    }

    public override void Recycle()
    {
		previousOscillation = 0f;
        ShapeBehaviorPool<OscillationShapeBehavior>.Reclaim(this);
    }

    public override bool GameUpdate(Shape shape)
    {
        float oscillation = Mathf.Sin(2f * Mathf.PI * Frequency * shape.Age);
		shape.transform.localPosition += (oscillation - previousOscillation) * Offset;
		previousOscillation = oscillation;
        return true;
    }
	
    public override void Save(GameDataWriter write)
    {
        write.Write(Offset);
		write.Write(Frequency);
		write.Write(previousOscillation);
    }

    public override void Load(GameDataReader reader)
    {
        Offset = reader.ReadVector();
		Frequency = reader.ReadFloat();
		previousOscillation = reader.ReadFloat();
    }
}
