using UnityEngine;
using UnityEditor;

namespace JakePerry
{
    [CustomPropertyDrawer(typeof(DamageData))]
    public class DamageDataPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            return singleLineHeight * 3;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Draw border
            Rect indRect = EditorGUI.IndentedRect(position);
            Color borderColor = new Color(0, 0, 0, 0.2f);

            EditorGUI.DrawRect(new Rect(indRect.x, indRect.y, indRect.width, 1f), borderColor);
            EditorGUI.DrawRect(new Rect(indRect.x, indRect.y, 1f, indRect.height), borderColor);
            EditorGUI.DrawRect(new Rect(indRect.x + indRect.width, indRect.y, 1f, indRect.height), borderColor);
            EditorGUI.DrawRect(new Rect(indRect.x, indRect.y + indRect.height, indRect.width, 1f), borderColor);

            // Draw label
            EditorGUI.LabelField(position, label, EditorStyles.miniLabel);

            // Draw properties
            Rect propPos = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            propPos.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(propPos, property.FindPropertyRelative("damageValue"));

            propPos.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(propPos, property.FindPropertyRelative("mask"));
        }
    }
}
