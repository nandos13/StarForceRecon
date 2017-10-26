using System.Collections.Generic;
using UnityEngine;
using InControl;

namespace JakePerry
{
    public interface IGameController
    {
        void UpdateController();
    }

    public abstract class GameController<T> : IGameController where T : GameSpecificInputSet, new()
    {
        #region Owned interface ITarget, enum ControlType

        /// <summary>Interface for anything which is able to receive input via a GameController.</summary>
        public interface ITarget
        {
            void ReceiveControllerInput(T inputSet, ControlType controllerType);
        }

        public enum ControlType
        {
            None = -1,
            KeyboardAndMouse = 0,
            Gamepad = 1,
        }
        
        #endregion

        #region Private references

        private GameControllerBehaviour behaviour = null;
        private List<ITarget> targets = new List<ITarget>();
        
        protected T inputSet = null;
        protected ControlType type = ControlType.None;

        #endregion

        public GameController()
        {
            // Create input set
            inputSet = new T();

            // Create behaviour script
            behaviour = GameControllerBehaviour.Create(this);
        }

        public GameController(ITarget target) : this()
        {
            // Store target reference
            targets.Add(target);
        }

        static GameController()
        {
            // Initialize InControl
            if (GameObject.FindObjectOfType<InControlManager>() == null)
            {
                InControlManager manager = InControlManager.Instance;
            }
        }

        /// <summary>Adds a new target to the controller.</summary>
        public void AddTarget(ITarget target)
        {
            if (target != null)
            {
                if (!targets.Contains(target))
                    targets.Add(target);
            }
        }

        /// <summary>Removes target from this controller.</summary>
        public void RemoveTarget(ITarget target)
        {
            if (target != null)
            {
                if (targets.Contains(target))
                    targets.Remove(target);
            }
        }

        /// <summary>Override to provide additional functionality to the end of Update call.</summary>
        protected virtual void OnUpdate()
        { }

        void IGameController.UpdateController()
        {
            foreach (ITarget target in targets)
            {
                if (target == null) continue;

                // Send updated controller info to target
                target.ReceiveControllerInput(inputSet, type);
            }

            OnUpdate();
        }

        /// <summary>Override to provide additional functionality to the end of Destroy() method.</summary>
        protected virtual void OnDestroy()
        { }

        /// <summary>Destroys the GameController.</summary>
        public void Destroy()
        {
            if (behaviour)
                GameObject.Destroy(behaviour.gameObject);

            if (inputSet != null)
                inputSet.Destroy();

            OnDestroy();
        }
    }

    /// <summary>Internal use by GameController only!</summary>
    internal class GameControllerBehaviour : MonoBehaviour
    {
        IGameController controller = null;

        public static GameControllerBehaviour Create(IGameController controller)
        {
            if (controller == null)
                throw new System.Exception("Cannot create a GameControllerBehaviour with null controller reference.");

            GameObject behaviourObject = new GameObject("GameController Updater");
            behaviourObject.hideFlags = HideFlags.HideAndDontSave;

            GameControllerBehaviour behaviour = behaviourObject.AddComponent<GameControllerBehaviour>();
            behaviour.controller = controller;
            return behaviour;
        }

        private void Update()
        {
            if (controller == null)
            {
                Destroy(this.gameObject);
                return;
            }

            controller.UpdateController();
        }
    }
}
