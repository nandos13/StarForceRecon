using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JakePerry
{
    [CustomEditor(typeof(DamageLayerUtils.DamageLayerNameFile))]
    public class DamageLayerNameFileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
