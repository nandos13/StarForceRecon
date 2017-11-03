using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDestructable : MonoBehaviour, Interaction.IInteractable
{
    #region Variables
    [Tooltip("If true, interacting character will become unuseable while the interaction is happening.")]
    [SerializeField]
    private bool lockCharacter = true;

    [Tooltip("Duration in seconds the interaction will take to complete.")]
    [SerializeField, Range(0.0f, 120.0f)]
    private float duration = 0.0f;
    
    [SerializeField]
    private Vector3 focusPointOffset = Vector3.zero;

    [Tooltip("The GameObject which will be disabled when the interaction is done.")]
    [SerializeField]
    private GameObject disablePiece;

    [Tooltip("The GameObject which will be enabled when the interaction is done.")]
    [SerializeField]
    private GameObject enablePiece;
    #endregion

    #region IInteractable Methods
    float Interaction.IInteractable.Duration { get { return duration; } }

    Vector3 Interaction.IInteractable.FocusPoint { get { return transform.TransformPoint(focusPointOffset); } }

    Interaction.InteractionType Interaction.IInteractable.Type { get { return Interaction.InteractionType.DestructableEnvironment; } }

    bool Interaction.IInteractable.ForceCharacterSwap { get { return lockCharacter; } }

    void Interaction.IInteractable.OnCompleteInteraction()
    {
        if (disablePiece)
            disablePiece.SetActive(false);

        if (enablePiece)
            enablePiece.SetActive(true);
    }

    void Interaction.IInteractable.OnStartInteraction(Interaction.InteractionInfo info)
    { }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((this as Interaction.IInteractable).FocusPoint, 0.4f);
    }
}
