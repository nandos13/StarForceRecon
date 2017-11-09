using UnityEngine;
using UnityEditor;
using System.Linq;

namespace JakePerry
{
    [CustomPropertyDrawer(typeof(DamageLayer.Mask))]
    public class DamageMaskPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string[] names = DamageLayer.Utils.GetLayerNames().Select((name, index) => string.Format("{0}. {1}", index, name)).ToArray();
            var maskValue = property.FindPropertyRelative("_mask");

            int currentIndex = maskValue.intValue;
            currentIndex = EditorGUI.MaskField(position, label, currentIndex, names);
            maskValue.intValue = currentIndex;
        }
    }
}
