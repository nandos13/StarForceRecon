using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class AimHandler : MonoBehaviour
{
    #region Variables

    #region Target Masking

    [SerializeField]
    private LayerMask aimableLayers = (LayerMask)1;

    [Tooltip("Tags ignored by smart aim. If the cursor is over a transform with contained tag, finding an optimal aim point will not be attempted.")]
    [SerializeField]
    private string[] smartAimIgnoreTags = new string[] { "Untagged" };
    [Tooltip("Tags which can be aimed at. All other tags will be ignored.")]
    [SerializeField]
    private string[] _aimTags = new string[] { "Untagged", "Enemy" };

    #endregion

    #endregion

    #region Functionality

    /// <summary>Handles aiming based on a viewport cursor position.</summary>
    public void HandlePlayerAiming(Vector2 viewportCoordinates)
    {
        Vector3 desiredTarget = GetDesiredTargetFromViewport(viewportCoordinates);

        // TODO
    }

    /// <summary>Calculates an aim target from a viewport location.</summary>
    public Vector3 GetDesiredTargetFromViewport(Vector2 viewportCoordinates)
    {
        Camera cam = Camera.main;

    }

    #endregion
}
