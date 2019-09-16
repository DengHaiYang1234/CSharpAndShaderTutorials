
using UnityEngine;


//继承PropertyAttribute这个类，便能使用一个自定义的PropertyDrawer的类去控制继承PropertyAttribute类的变量在Inspector面板的显示
public sealed class FloatRangeSliderAttribute : PropertyAttribute
{
    public float Min { get; private set; }
    public float Max { get; private set; }

    public FloatRangeSliderAttribute(float min,float max)
    {
        if (max < min)
        {
            max = min;
        }

        Min = min;
        Max = max;
    }
}
