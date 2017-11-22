using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GunData))]
public class GunDataEditor : Editor
{
    private void DrawInspector()
    {
        EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

        {
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel += 1;

            SerializedProperty damageProperty = serializedObject.FindProperty("_damage");
            if (damageProperty != null)
                EditorGUILayout.PropertyField(damageProperty);

            SerializedProperty spreadProperty = serializedObject.FindProperty("_spread");
            if (spreadProperty != null)
                EditorGUILayout.PropertyField(spreadProperty);

            EditorGUI.indentLevel = indent;
        }

        EditorGUILayout.LabelField("Firing", EditorStyles.boldLabel);

        {
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel += 1;

            SerializedProperty semiAutoProperty = serializedObject.FindProperty("_semiAuto");
            if (semiAutoProperty != null)
                EditorGUILayout.PropertyField(semiAutoProperty);

            SerializedProperty fireRateRPMProperty = serializedObject.FindProperty("_fireRateRPM");
            if (fireRateRPMProperty != null)
                EditorGUILayout.PropertyField(fireRateRPMProperty);

            EditorGUI.indentLevel = indent;
        }

        EditorGUILayout.LabelField("Ammo", EditorStyles.boldLabel);

        {
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel += 1;

            SerializedProperty bottomlessClipProperty = serializedObject.FindProperty("_bottomlessClip");
            if (bottomlessClipProperty != null)
            {
                EditorGUILayout.PropertyField(bottomlessClipProperty);

                if (!bottomlessClipProperty.boolValue)
                {
                    SerializedProperty clipSizeProperty = serializedObject.FindProperty("_clipSize");
                    if (clipSizeProperty != null)
                        EditorGUILayout.PropertyField(clipSizeProperty);
                }
            }

            SerializedProperty infiniteAmmoProperty = serializedObject.FindProperty("_infiniteAmmo");
            if (infiniteAmmoProperty != null)
                EditorGUILayout.PropertyField(infiniteAmmoProperty);

            SerializedProperty ammoPerShotProperty = serializedObject.FindProperty("_ammoPerShot");
            if (ammoPerShotProperty != null)
                EditorGUILayout.PropertyField(ammoPerShotProperty);

            SerializedProperty reloadTimeProperty = serializedObject.FindProperty("_reloadTime");
            if (reloadTimeProperty != null)
                EditorGUILayout.PropertyField(reloadTimeProperty);

            EditorGUI.indentLevel = indent;
        }

        SerializedProperty useHeatProperty = serializedObject.FindProperty("_useHeat");
        if (useHeatProperty != null)
        {
            Rect r = EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useHeatProperty, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUI.LabelField(new Rect(r.x + 13, r.y, r.width - 13, EditorGUIUtility.singleLineHeight),
                                "Heat", EditorStyles.boldLabel);

            if (useHeatProperty.boolValue)
            {
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel += 1;

                SerializedProperty heatLocksReloadProperty = serializedObject.FindProperty("_heatLocksReload");
                if (heatLocksReloadProperty != null)
                    EditorGUILayout.PropertyField(heatLocksReloadProperty);

                SerializedProperty overheatThresholdProperty = serializedObject.FindProperty("_overheatThreshold");
                if (overheatThresholdProperty != null)
                    EditorGUILayout.PropertyField(overheatThresholdProperty);

                SerializedProperty coolThresholdProperty = serializedObject.FindProperty("_coolThreshold");
                if (coolThresholdProperty != null)
                    EditorGUILayout.PropertyField(coolThresholdProperty);

                SerializedProperty heatPerShotProperty = serializedObject.FindProperty("_heatPerShot");
                if (heatPerShotProperty != null)
                    EditorGUILayout.PropertyField(heatPerShotProperty);

                SerializedProperty heatLossPerSecondProperty = serializedObject.FindProperty("_heatLossPerSecond");
                if (heatLossPerSecondProperty != null)
                    EditorGUILayout.PropertyField(heatLossPerSecondProperty);

                SerializedProperty coolingPauseTimeProperty = serializedObject.FindProperty("_coolingPauseTime");
                if (coolingPauseTimeProperty != null)
                    EditorGUILayout.PropertyField(coolingPauseTimeProperty);

                EditorGUI.indentLevel = indent;
            }
        }

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawInspector();

        serializedObject.ApplyModifiedProperties();
    }
}
