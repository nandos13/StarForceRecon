using UnityEngine;
using UnityEditor;

namespace JakePerry
{
    [CustomPropertyDrawer(typeof(DamageLayer.Mask))]
    public class DamageMaskPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string[] names = DamageLayerUtils.GetLayerNames();
            var maskValue = property.FindPropertyRelative("_mask");

            int currentIndex = maskValue.intValue;
            currentIndex = EditorGUI.MaskField(position, label, currentIndex, names);
            maskValue.intValue = currentIndex;
        }
    }
}
