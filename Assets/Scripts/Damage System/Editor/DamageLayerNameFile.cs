using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JakePerry
{
    [CustomEditor(typeof(DamageLayerDefinition))]
    public class DamageLayerNameFileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
