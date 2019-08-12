using UnityEngine;

public enum ShapeBehaviorType
{
    Movement, Rotation, Oscillation
}

//添加枚举方法
public static class ShapeBehaviorTypeMethods
{
    public static ShapeBehavior GetInstance(this ShapeBehaviorType type)
    {
        switch (type)
        {
            case ShapeBehaviorType.Movement:
                return ShapeBehaviorPool<MovementShapeBehavior>.Get();
            case ShapeBehaviorType.Rotation:
                return ShapeBehaviorPool<RotationShapeBehavior>.Get();
            case ShapeBehaviorType.Oscillation:
                return ShapeBehaviorPool<OscillationShapeBehavior>.Get();
        }
		Debug.LogError("Forgot to support" + type);
		return null;
    }
}

//Shape行为
public abstract class ShapeBehavior
#if UNITY_EDITOR
: ScriptableObject
#endif
{
    public abstract ShapeBehaviorType BehaviorType { get; }

    public abstract void Recycle();

    public abstract void GameUpdate(Shape shape);

    public abstract void Save(GameDataWriter write);

    public abstract void Load(GameDataReader reader);
#if UNITY_EDITOR
    public bool IsReclaimed { get; set; }

    void OnEnable()
    {
        if (IsReclaimed)
            Recycle();
    }
#endif
}


