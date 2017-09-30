using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageLayerDefinition : ScriptableObject
{
    [SerializeField]
    private string[] _strings = new string[32];

    public string[] strings
    {
        get { return _strings; }
        set { _strings = value; }
    }
}
