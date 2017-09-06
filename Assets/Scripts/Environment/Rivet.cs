using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rivet : MonoBehaviour, IRivetable
{
    [SerializeField]    private Component[] _components;
    [SerializeField]    private bool _destroyThisObject = false;

    /// <summary>Remove the rivet components.</summary>
    public void RemoveRivet()
    {
        foreach (Component c in _components)
        {
            if (c != null)
                Destroy(c);
        }

        if (_destroyThisObject)
            Destroy(this.gameObject);
    }
}
