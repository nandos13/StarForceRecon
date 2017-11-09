using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction
{
    #region Interfaces & Type Enum
    /// <summary>Interface for characters to interact with environment.</summary>
    public interface IInteractor
    {
        /// <summary>Called when the interaction starts.</summary>
        void OnStartInteraction(InteractionInfo info);
        /// <summary>Called when the interaction is completed.</summary>
        void OnCompleteInteraction();
    }

    /// <summary>Interface for interactable objects within the environment.</summary>
    public interface IInteractable
    {
        /// <summary>Should the character become unplayable whilst interacting?</summary>
        bool ForceCharacterSwap { get; }
        /// <summary>Duration in seconds to complete the interaction.</summary>
        float Duration { get; }
        /// <summary>The world-space focus point of this object.</summary>
        Vector3 FocusPoint { get; }
        /// <summary>The type of this interaction.</summary>
        InteractionType Type { get; }
        /// <summary>To be executed when the interaction starts.</summary>
        void OnStartInteraction(InteractionInfo info);
        /// <summary>To be executed when the interaction completes.</summary>
        void OnCompleteInteraction();
    }

    public enum InteractionType
    {
        DoorControlPanel,
        DoorJammed,
        DestructableEnvironment,
        HiveDestruct,
        AmmoBox,
        HealthStation
    }
    #endregion

    #region Info
    public class InteractionInfo
    {
        private bool complete;
        private float duration;
        private float elapsed;

        public bool Complete { get { return complete; } }
        public float Duration { get { return duration; } }
        public float Elapsed
        {
            get { return elapsed; }
            set
            {
                elapsed = value;
                if (elapsed >= duration)
                {
                    elapsed = duration;
                    complete = true;
                }
            }
        }
        public float Progress { get { return elapsed / duration; } }

        public IInteractor Owner { get; private set; }
        public IInteractable Target { get; private set; }

        public InteractionInfo(float duration, IInteractor owner, IInteractable target)
        {
            if (duration < 0) throw new System.Exception("InteractionInfo requires positive duration value.");
            if (owner == null || target == null) throw new System.ArgumentNullException("InteractionInfo requires non-null owner & target.");

            this.complete = false;
            this.duration = duration;
            this.elapsed = 0.0f;

            this.Owner = owner;
            this.Target = target;
        }
    }
    #endregion

    #region Functionality
    private Interaction()
    { }

    #region static
    private static Interaction instance;
    private static Coroutiner coroutiner;
    static Interaction()
    {
        instance = new Interaction();
        coroutiner = Coroutiner.Create();
    }

    public static void StartInteraction(IInteractor owner, IInteractable target)
    {
        if (owner == null || target == null) throw new System.ArgumentNullException("Cannot start an interaction with null owner or target.");
        instance.StartInteractionInternal(owner, target);
    }
    #endregion

    private void StartInteractionInternal(IInteractor owner, IInteractable target)
    {
        InteractionInfo info = new InteractionInfo(target.Duration, owner, target);

        owner.OnStartInteraction(info);
        target.OnStartInteraction(info);
        
        coroutiner.StartCoroutine(InteractionTracker(info));
    }

    private void CompleteInteraction(IInteractor owner, IInteractable target)
    {
        owner.OnCompleteInteraction();
        target.OnCompleteInteraction();
    }

    private IEnumerator InteractionTracker(InteractionInfo info)
    {
        yield return null;

        float duration = info.Duration;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        CompleteInteraction(info.Owner, info.Target);
    }
    #endregion

    #region Coroutiner
    private class Coroutiner : MonoBehaviour
    {
        public static Coroutiner Create()
        {
            GameObject obj = new GameObject("Interaction Coroutiner");
            obj.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(obj);
            return obj.AddComponent<Coroutiner>();
        }
    }
    #endregion
}
