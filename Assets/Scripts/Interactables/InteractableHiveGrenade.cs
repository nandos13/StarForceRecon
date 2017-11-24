using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableHiveGrenade : MonoBehaviour, Interaction.IInteractable
{
    #region Variables
    [Tooltip("If true, interacting character will become unuseable while the interaction is happening.")]
    [SerializeField]
    private bool lockCharacter = true;

    [Tooltip("Duration in seconds to drop the grenade.")]
    [SerializeField, Range(0.0f, 120.0f)]
    private float placementDuration = 0.0f;

    [Tooltip("Time in seconds before the grenade will explode.")]
    [SerializeField, Range(1.0f, 10.0f)]
    private float grenadeFuseTime = 3.0f;

    [SerializeField]
    private SpawnNode nodeToDestroy = null;

    [SerializeField]
    private ParticleSystem explosion = null;

    [SerializeField, Range(0.1f, 5.0f)]
    private float particleLifeTime = 1.0f;

    [SerializeField]
    private Vector3 focusPointOffset = Vector3.zero;

    [Tooltip("The GameObject which will be disabled when the interaction is done.")]
    [SerializeField]
    private GameObject disablePiece;

    [Tooltip("The GameObject which will be enabled when the interaction is done.")]
    [SerializeField]
    private GameObject enablePiece;
    #endregion

    private void Awake()
    {
        if (explosion != null)
            explosion.gameObject.SetActive(false);
    }

    #region IInteractable Methods
    float Interaction.IInteractable.Duration { get { return placementDuration; } }

    Vector3 Interaction.IInteractable.FocusPoint { get { return transform.TransformPoint(focusPointOffset); } }

    Interaction.InteractionType Interaction.IInteractable.Type { get { return Interaction.InteractionType.HiveDestruct; } }

    bool Interaction.IInteractable.ForceCharacterSwap { get { return lockCharacter; } }

    void Interaction.IInteractable.OnCompleteInteraction()
    {
        StartCoroutine(GrenadeExplosionTimer(grenadeFuseTime));
    }

    private IEnumerator GrenadeExplosionTimer(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (nodeToDestroy != null)
            Destroy(nodeToDestroy);

        if (disablePiece)
            disablePiece.SetActive(false);

        if (enablePiece)
            enablePiece.SetActive(true);

        if (explosion != null)
        {
            explosion.gameObject.SetActive(true);
            explosion.Play();
            StartCoroutine(ExplosionLifetime(particleLifeTime));
        }
        else
            Destroy(this);
    }

    private IEnumerator ExplosionLifetime(float time)
    {
        yield return new WaitForSeconds(time);
        explosion.gameObject.SetActive(false);
        Destroy(this);
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
