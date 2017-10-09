using UnityEngine;
using UnityEditor;
using System.Linq;

namespace JakePerry
{
    [CustomPropertyDrawer(typeof(DamageLayer))]
    public class DamageLayerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Get layer names, then convert to a GUIContent array
            string[] names = DamageLayer.Utils.GetLayerNames();
            GUIContent[] guicNames = new GUIContent[names.Length];
            guicNames = names.Select(name => new GUIContent(name)).ToArray();
            
            var layerValue = property.FindPropertyRelative("_layer");

            int currentIndex = layerValue.intValue;
            currentIndex = EditorGUI.Popup(position, label, currentIndex, guicNames);
            layerValue.intValue = currentIndex;
        }
    }
}
