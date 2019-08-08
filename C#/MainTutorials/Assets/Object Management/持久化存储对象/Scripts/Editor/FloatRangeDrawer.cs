
using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(FloatRange))]
public class FloatRangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int originalIndentLevel = EditorGUI.indentLevel;
        float originalLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, EditorGUIUtility.GetControlID(FocusType.Passive),label);
        position.width = position.width/2f;
        EditorGUIUtility.labelWidth = position.width/2f;
        EditorGUI.indentLevel = 1;
        EditorGUI.PropertyField(position,property.FindPropertyRelative("min"));
        position.x += position.width;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("max"));
        EditorGUI.EndProperty();
        EditorGUI.indentLevel = originalIndentLevel;
        EditorGUIUtility.labelWidth = originalLabelWidth;
    }

}
