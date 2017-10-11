using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

namespace JakePerry
{
    [CustomPropertyDrawer(typeof(DamageLayer.Modifier)), 
        DisallowMultipleComponent]
    public class DamageLayerModifierPropertyDrawer : PropertyDrawer
    {
        private bool foldoutOpen = false;
        private int newLayerSelectionIndex = 0;
        private float newLayerValue = 0.0f;

        const float BUTTON_PADDING = 10.0f;
        private const float MIDDLE_PADDING = 14.0f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            object modObj = fieldInfo.GetValue(property.serializedObject.targetObject);
            DamageLayer.Modifier mod = (DamageLayer.Modifier)modObj;
            var modifiers = mod.modifiers;
            int count = (modifiers == null) ? 0 : modifiers.Count;

            float baseHeight = EditorGUIUtility.singleLineHeight;
            float contentHeight = foldoutOpen ? EditorGUIUtility.singleLineHeight * count : 0;
            float newLayerHeight = EditorGUIUtility.singleLineHeight;
            
            return baseHeight + contentHeight + (foldoutOpen ? newLayerHeight + MIDDLE_PADDING + EditorGUIUtility.singleLineHeight : 0);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            foldoutOpen = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                foldoutOpen, label);
            if (foldoutOpen)
            {
                position.y += EditorGUIUtility.singleLineHeight;
                position = EditorGUI.IndentedRect(position);
                Draw(position, property, label);
            }
        }

        private void Draw(Rect position, SerializedProperty property, GUIContent label)
        {
            object modObj = fieldInfo.GetValue(property.serializedObject.targetObject);
            DamageLayer.Modifier mod = (DamageLayer.Modifier)modObj;
            mod.InitializeDict();
            var modifiers = mod.modifiers;

            #region Draw Contained Layers

            // Draw each contained layer & modifier value
            bool removeKey = false;
            DamageLayer keyToRemove = 0;
            foreach (KeyValuePair<DamageLayer, float> pair in modifiers)
            {
                // Draw layer name label
                EditorGUI.LabelField(new Rect(position.x, position.y,
                                        position.width / 2, EditorGUIUtility.singleLineHeight),
                                        DamageLayer.Utils.GetLayerName(pair.Key, true), EditorStyles.miniBoldLabel);

                // Draw modifier value
                float value = pair.Value;
                value = EditorGUI.FloatField(new Rect(position.x + position.width / 2, position.y,
                                            position.width / 4, EditorGUIUtility.singleLineHeight),
                                            value);

                // Set value if changed
                if (value != pair.Value)
                    modifiers[pair.Key] = value;

                // Draw remove button
                if (GUI.Button(new Rect(position.x + ((position.width / 4) * 3) + BUTTON_PADDING, position.y,
                    (position.width / 4) - 2 * BUTTON_PADDING, EditorGUIUtility.singleLineHeight), "-"))
                {
                    removeKey = true;
                    keyToRemove = pair.Key;
                }
                
                position.y += EditorGUIUtility.singleLineHeight;
            }

            if (removeKey)
                mod.RemoveModifier(keyToRemove);

            #endregion

            #region Spacer Line

            position.y += MIDDLE_PADDING / 2;
            EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, 1.0f), Color.black);
            position.y += MIDDLE_PADDING / 2;

            #endregion

            #region Draw Add New Layer

            // Get a list of layer names which do not already have a modifier
            string[] namesNotContained = DamageLayer.Utils.GetLayerNames(false).Where(
                name => !modifiers.ContainsKey(DamageLayer.Utils.NameToLayer(name))
                ).ToArray();

            // Draw fields for adding a new layer modifier
            if (namesNotContained.Length > 0)
            {
                if (newLayerSelectionIndex >= namesNotContained.Length)
                    newLayerSelectionIndex = 0;

                // Draw popup for adding a new layer
                newLayerSelectionIndex =
                    EditorGUI.Popup(new Rect(position.x, position.y, position.width / 2, EditorGUIUtility.singleLineHeight),
                        newLayerSelectionIndex, namesNotContained);

                // Draw float field for modifier value
                newLayerValue =
                    EditorGUI.FloatField(new Rect(position.x + position.width / 2, position.y, position.width / 4, EditorGUIUtility.singleLineHeight),
                    newLayerValue);

                // Draw button to add the selected layer
                if (GUI.Button(new Rect(position.x + ((position.width / 4) * 3) + BUTTON_PADDING, position.y, 
                    (position.width / 4) - 2 * BUTTON_PADDING, EditorGUIUtility.singleLineHeight), "+"))
                {
                    string nameToAdd = namesNotContained[newLayerSelectionIndex];
                    mod.SetModifier(DamageLayer.Utils.NameToLayer(nameToAdd), newLayerValue);

                    newLayerSelectionIndex = 0;
                }
            }
            else
                EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                    "Already contains all valid layers", EditorStyles.miniLabel);

            #endregion

            // Apply any changes
            modObj = mod as object;
            fieldInfo.SetValue(property.serializedObject.targetObject, modObj);
        }
    }
}
