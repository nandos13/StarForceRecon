using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

namespace JakePerry
{
    [CustomPropertyDrawer(typeof(DamageLayer.Modifier))]
    public class DamageLayerModifierPropertyDrawer : PropertyDrawer
    {
        private bool foldoutOpen = true;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + (foldoutOpen ? 
                EditorGUIUtility.singleLineHeight * property.FindPropertyRelative("modifierKeys").arraySize : 0);
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
            var keysProperty = property.FindPropertyRelative("modifierKeys");
            if (keysProperty != null)
            {
                // Get all layers which have a modifier
                int keyCount = keysProperty.arraySize;
                List<int> keys = new List<int>();
                if (keyCount > 0)
                {
                    // Get all key layer values
                    int i = 0;
                    while (keyCount > i)
                    {
                        keys.Add(keysProperty.GetArrayElementAtIndex(i).FindPropertyRelative("_layer").intValue);
                        i++;
                    }

                    // Get a list of layer names which do not already have a modifier
                    string[] namesNotContained = DamageLayer.Utils.GetLayerNames().Where(
                        name => !keys.Contains(DamageLayer.Utils.NameToLayer(name))
                        ).ToArray();

                    // Draw modifier property for each contained key
                    SerializedProperty values = property.FindPropertyRelative("modifierValues");
                    i = 0;
                    while (i < keys.Count)
                    {
                        int key = keys[i];

                        // Draw layer name label
                        EditorGUI.LabelField(new Rect(position.x, position.y, 
                                                position.width / 3, EditorGUIUtility.singleLineHeight),
                                                DamageLayer.Utils.GetLayerName(key), EditorStyles.miniBoldLabel);

                        // Draw modifier value
                        values.GetArrayElementAtIndex(i).floatValue =
                            EditorGUI.FloatField(new Rect(position.x + position.width / 3, position.y, 
                                                    position.width / 3, EditorGUIUtility.singleLineHeight),
                                                    values.GetArrayElementAtIndex(i).floatValue);

                        i++;
                        position.y += EditorGUIUtility.singleLineHeight;
                    }
                }
            }
        }
    }
}
