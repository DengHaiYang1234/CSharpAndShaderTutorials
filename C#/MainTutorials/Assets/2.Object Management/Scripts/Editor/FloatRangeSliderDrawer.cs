using UnityEditor;
using UnityEngine;

//使用FloatRangeDrawer的GUI
//[CustomPropertyDrawer(typeof(FloatRangeDrawer))]

//求区间值，类似Range（0，1）
[CustomPropertyDrawer(typeof(FloatRangeSliderAttribute))]
public class FloatRangeSliderDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //使用FloatRangeDrawer的GUI
        //base.OnGUI(position,property,label);

        int originalIndentLevel = EditorGUI.indentLevel;
		EditorGUI.BeginProperty(position, label, property);

        //获取变量名
		position = EditorGUI.PrefixLabel(
			position, GUIUtility.GetControlID(FocusType.Passive), label
		);

		EditorGUI.indentLevel = 0;
        //获取property的min和max
		SerializedProperty minProperty = property.FindPropertyRelative("min");
		SerializedProperty maxProperty = property.FindPropertyRelative("max");

        //获取值
		float minValue = minProperty.floatValue;
		float maxValue = maxProperty.floatValue;
		float fieldWidth = position.width / 4f - 4f;
		float sliderWidth = position.width / 2f;
		position.width = fieldWidth;
		minValue = EditorGUI.FloatField(position, minValue);
		position.x += fieldWidth + 4f;
		position.width = sliderWidth;
		FloatRangeSliderAttribute limit = attribute as FloatRangeSliderAttribute;
        //画Slider
		EditorGUI.MinMaxSlider(
			position, ref minValue, ref maxValue, limit.Min, limit.Max
		);

		position.x += sliderWidth + 4f;
		position.width = fieldWidth;
		maxValue = EditorGUI.FloatField(position, maxValue);
		if (minValue < limit.Min) {
			minValue = limit.Min;
		}
		if (maxValue < minValue) {
			maxValue = minValue;
		}
		else if (maxValue > limit.Max) {
			maxValue = limit.Max;
		}
        
		minProperty.floatValue = minValue;
		maxProperty.floatValue = maxValue;

		EditorGUI.EndProperty();
		EditorGUI.indentLevel = originalIndentLevel;



        // int originalIndentLevel = EditorGUI.indentLevel;
        // EditorGUI.BeginProperty(position, label, property);
        // //位置调整
        // position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        // EditorGUI.indentLevel = 0;
        // //返回一个
        // SerializedProperty minProperty = property.FindPropertyRelative("min");
        // SerializedProperty maxProperty = property.FindPropertyRelative("max");
        // float minValue = minProperty.floatValue;
        // float maxValue = maxProperty.floatValue;

        // float fieldWidth = position.width / 4f - 4f;
        // float sliderWidth = position.width / 2f;

        // position.width = fieldWidth;
        // minValue = EditorGUI.FloatField(position, minValue);
        // position.x += fieldWidth + 4;
        // position.width = sliderWidth;

        // FloatRangeSliderAttribute limit = attribute as FloatRangeSliderAttribute;
        // minProperty.floatValue = minValue;
        // maxProperty.floatValue = maxValue;

        // EditorGUI.MinMaxSlider(
        //     position, ref minValue, ref maxValue, limit.Min, limit.Max
        // );

        // position.x += sliderWidth + 4f;
        // position.width = fieldWidth;
        // maxValue = EditorGUI.FloatField(position, maxValue);

        // if (minValue < limit.Min)
        // {
        //     minValue = limit.Min;
        // }
        // if (maxValue < minValue)
        // {
        //     maxValue = minValue;
        // }
        // else if (maxValue > limit.Max)
        // {
        //     maxValue = limit.Max;
        // }

        // EditorGUI.EndProperty();
        // EditorGUI.indentLevel = originalIndentLevel;
    }
}
