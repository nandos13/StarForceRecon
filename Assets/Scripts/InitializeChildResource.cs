using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Loads a resource and creates an instance as a child of the transform.</summary>
public class InitializeChildResource : MonoBehaviour
{
    [SerializeField]
    private string resourcePath = string.Empty;
    [SerializeField]
    private RuntimeAnimatorController animController = null;

    private void Awake()
    {
        // Get resource
        GameObject modelObject = Resources.Load<GameObject>(resourcePath);
        if (modelObject != null)
        {
            // Instantiate the model as a child of this transform
            GameObject instanceObject = Instantiate<GameObject>(modelObject, this.transform);

            if (animController != null)
            {
                Animator animator = instanceObject.GetComponentInChildren<Animator>();
                if (animator != null)
                    animator.runtimeAnimatorController = animController;
            }
        }
        else
            Debug.LogError(string.Format("Failed to load asset at Resources/{0} as a GameObject.", resourcePath), this);

        Destroy(this);
    }
}
