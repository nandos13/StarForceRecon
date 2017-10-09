using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Gun))]
public class GunEditor : Editor
{

    private void DrawInspector()
    {
        EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

        #region General
        {
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel += 1;

            SerializedProperty originProperty = serializedObject.FindProperty("_gunOrigin");
            if (originProperty != null)
                EditorGUILayout.PropertyField(originProperty);

            SerializedProperty layerMaskProperty = serializedObject.FindProperty("_layerMask");
            if (layerMaskProperty != null)
                EditorGUILayout.PropertyField(layerMaskProperty);

            SerializedProperty damageMaskProperty = serializedObject.FindProperty("_damageMask");
            if (damageMaskProperty != null)
                EditorGUILayout.PropertyField(damageMaskProperty);

            SerializedProperty gunDataProperty = serializedObject.FindProperty("_gunData");
            if (gunDataProperty != null)
                EditorGUILayout.PropertyField(gunDataProperty);

            EditorGUI.indentLevel = indent;
        }
        #endregion

        EditorGUILayout.LabelField("Ammo", EditorStyles.boldLabel);

        #region Ammo
        {
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel += 1;

            SerializedProperty startAmmoProperty = serializedObject.FindProperty("_startAmmo");
            if (startAmmoProperty != null)
                EditorGUILayout.PropertyField(startAmmoProperty);

            EditorGUI.indentLevel = indent;
        }
        #endregion

        SerializedProperty heatProperty = serializedObject.FindProperty("_currentHeat");
        if (heatProperty != null)
        {
            GUIContent guic = new GUIContent("heat: " + heatProperty.floatValue.ToString());
            Rect rect = EditorGUI.IndentedRect(GUILayoutUtility.GetRect(guic, "TextField"));
            ProgressBar(rect, heatProperty.floatValue, guic.text);
        }

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawInspector();

        serializedObject.ApplyModifiedProperties();
    }

    private void ProgressBar(Rect rect, float value, string label)
    {
        EditorGUI.ProgressBar(rect, value, label);
        EditorGUILayout.Space();
    }
}
