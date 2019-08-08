using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FloatRangeDrawer))]
public class FloatRangeSliderDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);
    }

}
