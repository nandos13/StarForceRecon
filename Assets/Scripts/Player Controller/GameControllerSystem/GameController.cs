using UnityEngine;
using InControl;

namespace JakePerry
{
    public abstract class GameController
    {
        #region Owned interface ITarget, class Action Set, struct ActionState, enum ControlType

        /// <summary>Interface for anything which is able to receive input via a GameController.</summary>
        public interface ITarget
        {
            void ReceiveMoveInput(float horizontal, float vertical);
            void ReceiveAimInput(float horizontal, float vertical);
            void ReceiveAnalogAimInput(float horizontal, float vertical);
            void ReceiveActionInput(ActionState actionState);
        }

        protected class GameControlActionSet : PlayerActionSet
        {
            #region Movement Axes

            public PlayerAction Forward { get; private set; }
            public PlayerAction Back { get; private set; }
            public PlayerAction Left { get; private set; }
            public PlayerAction Right { get; private set; }

            public PlayerOneAxisAction Horizontal { get; private set; }
            public PlayerOneAxisAction Vertical { get; private set; }

            #endregion

            #region Look Axes

            public PlayerAction LookForward { get; private set; }
            public PlayerAction LookBack { get; private set; }
            public PlayerAction LookLeft { get; private set; }
            public PlayerAction LookRight { get; private set; }

            public PlayerOneAxisAction LookHorizontal { get; private set; }
            public PlayerOneAxisAction LookVertical { get; private set; }

            public PlayerAction LookForwardAnalog { get; private set; }
            public PlayerAction LookBackAnalog { get; private set; }
            public PlayerAction LookLeftAnalog { get; private set; }
            public PlayerAction LookRightAnalog { get; private set; }

            public PlayerOneAxisAction LookHorizontalAnalog { get; private set; }
            public PlayerOneAxisAction LookVerticalAnalog { get; private set; }

            #endregion

            #region Actions

            public PlayerAction Action1 { get; private set; }
            public PlayerAction Action2 { get; private set; }
            public PlayerAction Action3 { get; private set; }
            public PlayerAction Action4 { get; private set; }

            public PlayerAction LeftBumper { get; private set; }
            public PlayerAction RightBumper { get; private set; }

            public PlayerAction Start { get; private set; }
            public PlayerAction Select { get; private set; }

            public PlayerAction DPadUp { get; private set; }
            public PlayerAction DPadDown { get; private set; }
            public PlayerAction DPadLeft { get; private set; }
            public PlayerAction DPadRight { get; private set; }

            public PlayerAction Trigger1 { get; private set; }
            public PlayerAction Trigger2 { get; private set; }

            #endregion

            public GameControlActionSet()
            {
                // Create actions for Movement
                Forward = CreatePlayerAction("Forward");
                Back = CreatePlayerAction("Back");
                Left = CreatePlayerAction("Left");
                Right = CreatePlayerAction("Right");
                Horizontal = CreateOneAxisPlayerAction(Left, Right);
                Vertical = CreateOneAxisPlayerAction(Back, Forward);

                // Create actions for Look
                LookForward = CreatePlayerAction("LookForward");
                LookBack = CreatePlayerAction("LookBack");
                LookLeft = CreatePlayerAction("LookLeft");
                LookRight = CreatePlayerAction("LookRight");
                LookHorizontal = CreateOneAxisPlayerAction(LookLeft, LookRight);
                LookVertical = CreateOneAxisPlayerAction(LookBack, LookForward);

                LookForwardAnalog = CreatePlayerAction("LookForwardAnalog");
                LookBackAnalog = CreatePlayerAction("LookBackAnalog");
                LookLeftAnalog = CreatePlayerAction("LookLeftAnalog");
                LookRightAnalog = CreatePlayerAction("LookRightAnalog");
                LookHorizontalAnalog = CreateOneAxisPlayerAction(LookLeftAnalog, LookRightAnalog);
                LookVerticalAnalog = CreateOneAxisPlayerAction(LookBackAnalog, LookForwardAnalog);

                // Create actions for D-Pad
                DPadUp = CreatePlayerAction("DPadUp");
                DPadDown = CreatePlayerAction("DPadDown");
                DPadLeft = CreatePlayerAction("DPadLeft");
                DPadRight = CreatePlayerAction("DPadRight");

                // Create actions for Triggers
                Trigger1 = CreatePlayerAction("Trigger1");
                Trigger2 = CreatePlayerAction("Trigger2");

                // Create actions for Actions
                Action1 = CreatePlayerAction("Action1");
                Action2 = CreatePlayerAction("Action2");
                Action3 = CreatePlayerAction("Action3");
                Action4 = CreatePlayerAction("Action4");
                LeftBumper = CreatePlayerAction("LeftBumper");
                RightBumper = CreatePlayerAction("RightBumper");
                Start = CreatePlayerAction("Start");
                Select = CreatePlayerAction("Select");
            }
        }

        public struct ActionState
        {
            public PlayerAction Action1;
            public PlayerAction Action2;
            public PlayerAction Action3;
            public PlayerAction Action4;

            public PlayerAction LeftBumper;
            public PlayerAction RightBumper;

            public PlayerAction Start;
            public PlayerAction Select;

            public PlayerAction DPadUp;
            public PlayerAction DPadDown;
            public PlayerAction DPadLeft;
            public PlayerAction DPadRight;

            public PlayerAction Trigger1;
            public PlayerAction Trigger2;
        }

        public enum ControlType
        {
            KeyboardAndMouse,
            Gamepad,
        }

        #endregion

        #region Private references

        private GameControllerBehaviour behaviour = null;
        private ITarget target = null;

        protected GameControlActionSet actionSet = null;

        #endregion

        public GameController(ITarget target)
        {
            if (target == null)
                throw new System.Exception("GameController cannot be created with a null reference target.");

            // Store target reference
            this.target = target;

            // Create action set
            actionSet = new GameControlActionSet();

            // Create behaviour script
            behaviour = GameControllerBehaviour.Create(this);
        }

        static GameController()
        {
            // Initialize InControl
            if (GameObject.FindObjectOfType<InControlManager>() == null)
            {
                GameObject InControlManagerObject = new GameObject("InControlManager");
                InControlManagerObject.AddComponent<InControlManager>();
            }
        }

        /// <summary>Override to provide additional functionality to the end of Update call.</summary>
        protected virtual void OnUpdate()
        { }

        private void Update()
        {
            // Send updated controller info to target
            target.ReceiveMoveInput(actionSet.Horizontal.Value, actionSet.Vertical.Value);
            target.ReceiveAimInput(actionSet.LookHorizontal.Value, actionSet.LookVertical.Value);
            target.ReceiveAnalogAimInput(actionSet.LookHorizontalAnalog.Value, actionSet.LookVerticalAnalog.Value);
            target.ReceiveActionInput(GenerateActionState());

            OnUpdate();
        }

        private ActionState GenerateActionState()
        {
            ActionState state = new ActionState();

            state.Action1 = actionSet.Action1;
            state.Action2 = actionSet.Action2;
            state.Action3 = actionSet.Action3;
            state.Action4 = actionSet.Action4;

            state.LeftBumper = actionSet.LeftBumper;
            state.RightBumper = actionSet.RightBumper;

            state.Start = actionSet.Start;
            state.Select = actionSet.Select;

            state.DPadUp = actionSet.DPadUp;
            state.DPadDown = actionSet.DPadDown;
            state.DPadLeft = actionSet.DPadLeft;
            state.DPadRight = actionSet.DPadRight;

            state.Trigger1 = actionSet.Trigger1;
            state.Trigger2 = actionSet.Trigger2;

            return state;
        }

        /// <summary>Override to provide additional functionality to the end of Destroy() method.</summary>
        protected virtual void OnDestroy()
        { }

        /// <summary>Destroys the GameController.</summary>
        public void Destroy()
        {
            if (behaviour)
                GameObject.Destroy(behaviour.gameObject);

            if (actionSet != null)
                actionSet.Destroy();

            OnDestroy();
        }

        #region Behaviour

        private class GameControllerBehaviour : MonoBehaviour
        {
            GameController controller = null;

            public static GameControllerBehaviour Create(GameController controller)
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
                controller.Update();
            }
        }

        #endregion
    }
}
