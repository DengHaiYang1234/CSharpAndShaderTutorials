
using UnityEngine;
using UnityEditor;


//运行的是哪个类
[CustomPropertyDrawer(typeof(FloatRange))]
public class FloatRangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int originalIndentLevel = EditorGUI.indentLevel;
        float originalLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUI.BeginProperty(position, label, property);
        //只要使用FloatRange的变量，都将显示在面板上
        position = EditorGUI.PrefixLabel(position, EditorGUIUtility.GetControlID(FocusType.Passive),label);
        position.width = position.width/2f;
        //字段标签的缩进
        EditorGUIUtility.labelWidth = position.width/2f;
        EditorGUI.indentLevel = 1;
        //创建min属性
        EditorGUI.PropertyField(position,property.FindPropertyRelative("min"));
        position.x += position.width;
        //创建Max属性
        EditorGUI.PropertyField(position, property.FindPropertyRelative("max"));
        EditorGUI.EndProperty();
        EditorGUI.indentLevel = originalIndentLevel;
        EditorGUIUtility.labelWidth = originalLabelWidth;


    }
}
