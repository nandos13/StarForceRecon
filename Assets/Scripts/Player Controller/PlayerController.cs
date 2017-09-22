﻿using System.Collections.Generic;
using UnityEngine;
using JakePerry;

namespace StarForceRecon
{
    [RequireComponent(typeof(ThirdPersonController)), 
        RequireComponent(typeof(AimHandler)),
        RequireComponent(typeof(Equipment)),
        DisallowMultipleComponent]
    public class PlayerController : MonoBehaviour, GameController.ITarget, SquadManager.IControllable
    {
        #region Variables

        #region Input Controls

        private GameController _keyboardController = null;
        private GameController _gamepadController = null;
        private int _lastAimMouseTime = -1;
        private int _lastAimJoystickTime = -1;
        private int _validInputReceiveTime = -1;

        private Vector2 _moveDirection = Vector2.zero;
        private bool _rollKeyPressed = false;

        #endregion

        #region Inspector accessible

        [Header("Aiming")]

        [Tooltip("When aiming, the character will turn towards aimer when the angle from forward exceeds this value.")]
        [SerializeField, Range(20.0f, 80.0f)]
        private float maxHipSwivel = 50.0f;

        [Tooltip("Character's turning speed in rotations/second.")]
        [SerializeField, Range(1.0f, 2.0f)]
        private float turningSpeed = 1.0f;

        [Tooltip("When aiming at a point within this radius from the character, the character will be locked to melee attacking only.")]
        [SerializeField, Range(1.0f, 3.0f)]
        private float closeAimRadius = 0.5f;

        [Tooltip("Time in seconds for cursor to fade out when the character is not aiming.")]
        [SerializeField, Range(0.2f, 2.0f)]
        private float cursorFadeTime = 1.0f;

        [Header("Character Switching")]

        [Tooltip("A list of AI scripts which will be enabled when the character is not being controlled by the player")]
        [SerializeField]
        private List<MonoBehaviour> aiScripts;
        [Tooltip("A list of Player scripts which will be enabled when the character is being controlled by the player")]
        [SerializeField]
        private List<MonoBehaviour> playerScripts;

        [Tooltip("Delay in seconds before re-enabling Controller scripts.")]
        [SerializeField, Range(0.2f, 1.0f)]
        private float selectionDelay = 0.2f;

        #endregion

        #region Private State Trackers & References

        private ThirdPersonController _tpc = null;
        private AimHandler _aimHandler = null;
        private Equipment _equipment = null;

        private static readonly Vector2 CENTER_VIEWPORT = new Vector2(0.5f, 0.5f);
        private static readonly Vector3 HORIZON_SCALE_VECTOR = new Vector3(1, 0, 1);

        public bool aiming { get; private set; }
        public Vector3 aimPoint { get; private set; }
        private bool meleeLocked = false;

        private bool secondaryIsEquipped = false;

        private static Canvas cursorCanvas = null;
        private static UnityEngine.UI.Image cursorSprite = null;

        #endregion

        #endregion

        #region Functionality

        private void Awake()
        {
            // Initialize aimer canvas
            if (cursorCanvas == null)
                cursorCanvas = CreateCursor(out cursorSprite);

            // Initialize Input
            _keyboardController = new KeyboardMouseController(this);
            _gamepadController = new DualStickController(this);

            // Get component references
            _tpc = GetComponent<ThirdPersonController>();
            _aimHandler = GetComponent<AimHandler>();
            _equipment = GetComponent<Equipment>();

            if (_tpc == null)
                throw new System.MissingFieldException("No Third Person Controller component found.");
            if (_aimHandler == null)
                throw new System.MissingFieldException("No Aim Handler component found.");
            if (_equipment == null)
                throw new System.MissingFieldException("No Equipment component found.");
        }

        private void Start()
        {
            // Register to the Squad Manager
            SquadManager.AddSquadMember(this);
        }

        private Canvas CreateCursor(out UnityEngine.UI.Image cursorSprite)
        {
            GameObject canvasObject = new GameObject("Aiming Cursor");
            canvasObject.hideFlags = HideFlags.HideAndDontSave;
            GameObject cursorObject = new GameObject("Cursor sprite");
            cursorObject.transform.parent = canvasObject.transform;

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            UnityEngine.UI.CanvasScaler scaler = canvasObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 1f;

            UnityEngine.UI.Image image = cursorObject.AddComponent<UnityEngine.UI.Image>();
            image.sprite = Resources.Load<Sprite>("Sprites/AimCursor");

            cursorSprite = image;
            return canvas;
        }

        private void Update()
        {
            // Find aim point, limited to minimum radius
            aimPoint = _aimHandler.HandlePlayerAiming(StarForceRecon.Cursor.position);
            float horizontalDistance = Vector3.Distance(transform.position, new Vector3(aimPoint.x, 0, aimPoint.z));
            aiming = (horizontalDistance >= closeAimRadius);

            // Rotate towards aim
            if (aiming)
                RotateToFaceAimCheck(StarForceRecon.Cursor.position, maxHipSwivel);

            UpdateOnScreenCursor();
        }

        private void FixedUpdate()
        {
            MovePlayerCharacter();
        }

        /// <summary>Updates position, rotation & alpha of on screen cursor.</summary>
        private void UpdateOnScreenCursor()
        {
            // Match cursor sprite to cursor position
            cursorSprite.rectTransform.position =
                StarForceRecon.Cursor.ToScreenSpace(cursorCanvas.pixelRect.width, cursorCanvas.pixelRect.height);
            if (aiming)
                cursorSprite.rectTransform.rotation =
                    Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector3.up, StarForceRecon.Cursor.position - CENTER_VIEWPORT));

            // Set cursor alpha
            Color color = cursorSprite.color;

            if (!aiming) color.a -= Time.deltaTime / cursorFadeTime;
            else color.a = 1;

            color.a = Mathf.Clamp01(color.a);
            cursorSprite.color = color;
        }

        /// <summary>Moves the cursor on the screen based on given input.</summary>
        /// <param name="aimInput">The aiming input.</param>
        /// <param name="useJoystickAim">Should this input be treated as a joystick?</param>
        private void ModifyCursorPosition(Vector2 aimInput, bool useJoystickAim)
        {
            const float CONTROLLER_SPEED = 0.1f;   // (Lower is faster)    // TODO: Make a sensitivity slider
            const float MOUSE_SPEED = 100.0f; // (Lower is slower)    // TODO: Make a mouse sensitivity slider

            if (useJoystickAim)
            {
                //float joystickAngle = Vector2.SignedAngle(Vector2.up, aimInput);
                //if (joystickAngle < 0) joystickAngle += 360.0f;
                Vector2 destination = (CENTER_VIEWPORT + (aimInput / 2));   // Circular aiming

                StarForceRecon.Cursor.MoveTo(destination, CONTROLLER_SPEED, Time.deltaTime);
            }
            else
                StarForceRecon.Cursor.FixedMove(aimInput * MOUSE_SPEED, new Vector2(Screen.width, Screen.height));
        }

        /// <summary>Finds where the cursor is hovering over the character's horizontal plane.</summary>
        /// <param name="viewportCursorPosition">Cursor position in Viewport [0-1][0-1].</param>
        /// <param name="intersect">Out: Resulting intersection point.</param>
        /// <returns>True if the character's horizon plane is under the cursor.</returns>
        public bool CharacterHorizonIntersect(Vector2 viewportCursorPosition, out Vector3 intersect)
        {
            Plane characterHorizon = new Plane(Vector3.up, transform.position);
            Ray cursorRay = Camera.main.ViewportPointToRay(viewportCursorPosition);

            float intersectDistance;
            if (characterHorizon.Raycast(cursorRay, out intersectDistance))
            {
                intersect = cursorRay.GetPoint(intersectDistance);
                return true;
            }

            intersect = Vector3.zero;
            return false;
        }

        /// <summary>Ensures the character is always facing the aim cursor.</summary>
        /// <param name="viewportCursorPosition">Cursor position in Viewport [0-1][0-1].</param>
        /// <param name="maxAngleDifference">The maximum angle allowed between character forward and cursor point.</param>
        private void RotateToFaceAimCheck(Vector2 viewportCursorPosition, float maxAngleDifference)
        {
            // Find where the cursor hovers over the character's horizontal plane
            Vector3 cursorIntersectsPlane;
            if (CharacterHorizonIntersect(viewportCursorPosition, out cursorIntersectsPlane))
            {
                Vector3 cursorIntersectLocal = cursorIntersectsPlane - transform.position;
                // Is aim angle from character forward too large?
                float aimTheta = Vector3.Angle(transform.forward, cursorIntersectLocal);
                if (aimTheta > maxAngleDifference)
                {
                    // Rotate to face cursor
                    float turnSpeed = (turningSpeed * 360.0f) / aimTheta;
                    Quaternion rotation = Quaternion.LookRotation(cursorIntersectLocal);
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
                }
            }
        }
        
        /// <summary>Makes the character aim at a certain point based on input.</summary>
        private void AimPlayerCharacter(Vector2 aimInput, GameController.ControlType type)
        {
            bool useJoystickAim = false;

            if (type == GameController.ControlType.Gamepad)
            {
                // Use joystick aiming if controller has been used more recently than mouse
                useJoystickAim = (_lastAimMouseTime + 1) < _lastAimJoystickTime;

                if (aimInput.x != 0 || aimInput.y != 0) _lastAimJoystickTime = Time.frameCount;
            }
            else
                if (aimInput.x != 0 || aimInput.y != 0) _lastAimMouseTime = Time.frameCount;
            
            ModifyCursorPosition(aimInput, useJoystickAim);
        }

        /// <summary>Moves the character based on input since last move.</summary>
        private void MovePlayerCharacter()
        {
            Vector3 move = Vector3.zero;
            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector3 forward = Vector3.Scale(cam.transform.forward, HORIZON_SCALE_VECTOR).normalized;
                move = _moveDirection.y * forward + _moveDirection.x * cam.transform.right;

                // TODO: Get input for crouch
                // Move character, interrupt melee if necessary
                _tpc.Move(move, _rollKeyPressed);
                InterruptMeleeAttack();
            }

            // Reset input tracking to allow extra input to be received
            _validInputReceiveTime = -1;
            _rollKeyPressed = false;
        }

        private void InterruptMeleeAttack()
        {
            if (meleeLocked)
            {
                // TODO
                meleeLocked = false;
            }
        }

        /// <summary>Attempts to do a melee attack.</summary>
        private void StartMeleeAttack()
        {
            if (!meleeLocked)
            {
                // TODO
                //meleeLocked = true;
            }
        }

        #region GameController ITarget Methods

        void GameController.ITarget.ReceiveActionInput(GameController.ActionState actionState)
        {
            if (isActiveAndEnabled)
            {
                if (actionState.LeftBumper.WasPressed)
                {
                    SquadManager.Switch(true);
                    return;
                }

                if (actionState.RightBumper.WasPressed)
                {
                    SquadManager.Switch(false);
                    return;
                }

                if (actionState.Action1.WasPressed) // If ROLL key was pressed this frame
                    _rollKeyPressed = true;
                if (_rollKeyPressed || _tpc.isRolling) return; // Prevent other actions if rolling

                if (!meleeLocked && actionState.Action2.WasPressed) // If MELEE key was pressed this frame
                    StartMeleeAttack();
                if (meleeLocked) return; // Prevent other actions if doing a melee attack

                if (actionState.Action4.WasPressed) // If WEAPON_SWITCH key was pressed this frame
                {
                    _equipment.PrimarySecondarySwap();
                    // TODO: Disable firing until swap animation is done
                    return;
                }

                if (actionState.Trigger1.IsPressed)
                    _equipment.Use(Equipment.Slot.Primary);

                // TODO: Other equipment
            }
        }

        void GameController.ITarget.ReceiveAimInput(Vector2 aimInput, GameController.ControlType type)
        {
            if (isActiveAndEnabled)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Confined;
                UnityEngine.Cursor.visible = false;
                AimPlayerCharacter(aimInput, type);
            }
        }

        void GameController.ITarget.ReceiveMoveInput(Vector2 moveInput)
        {
            // Record input to be applied on next FixedUpdate call.
            // Once input has been received, input is locked until the next FixedUpdate.
            if (_validInputReceiveTime < 0)
            {
                _moveDirection.x = moveInput.x;
                _moveDirection.y = moveInput.y;

                _validInputReceiveTime = Time.frameCount;
            }
            else
            {
                // Take the largest of movement values
                float total = Mathf.Abs(moveInput.x) + Mathf.Abs(moveInput.y);
                float currentTotal = Mathf.Abs(_moveDirection.x) + Mathf.Abs(_moveDirection.y);

                if (total > currentTotal)
                {
                    _moveDirection.x += moveInput.x;
                    _moveDirection.y += moveInput.y;
                }
            }
        }

        #endregion

        #region SquadManager IControllable Methods

        private void SelectCharacter()
        {
            foreach (MonoBehaviour behaviour in aiScripts)
                behaviour.enabled = false;

            foreach (MonoBehaviour behaviour in playerScripts)
                behaviour.enabled = true;

            // Enable this script
            this.enabled = true;

            // Change cursor colour to this character's colour
            cursorSprite.color = Color.red;
        }

        void SquadManager.IControllable.OnSwitchAway()
        {
            CancelInvoke("SelectCharacter");

            foreach (MonoBehaviour behaviour in aiScripts)
                behaviour.enabled = true;

            foreach (MonoBehaviour behaviour in playerScripts)
                behaviour.enabled = false;

            // Disable this script
            this.enabled = false;
        }

        void SquadManager.IControllable.OnSwitchTo()
        {
            Invoke("SelectCharacter", selectionDelay);
        }

        Transform SquadManager.IControllable.transform
        { get { return transform; } }

        #endregion

        private void OnDisable()
        {
            _tpc.StopMovement();
        }

        #endregion
    }

    public static class Cursor
    {
        private static Vector2 pos = new Vector2(0.5f, 0.5f);
        /// <summary>Viewport position of the cursor [0-1][0-1].</summary>
        public static Vector2 position { get { return pos; } }

        private static void ClampPosition()
        {
            pos.x = Mathf.Clamp01(pos.x);
            pos.y = Mathf.Clamp01(pos.y);
        }

        /// <param name="viewportPosition">New position in viewport [0-1][0-1].</param>
        public static void SetPosition(Vector2 viewportPosition)
        {
            pos = viewportPosition;
            ClampPosition();
        }

        /// <summary>Smoothly move the cursor in a direction.</summary>
        /// <param name="direction">Direction to move the cursor (normalized).</param>
        /// <param name="borderTime">Time to move from one screen border to opposite border in seconds.</param>
        public static void Move(Vector2 direction, float borderTime, float deltaTime)
        {
            if (borderTime < 0) borderTime = -borderTime;
            direction.Normalize();
            
            pos += direction * (deltaTime / borderTime);
            ClampPosition();
        }

        /// <summary>Smoothly move the cursor to a particular point.</summary>
        /// <param name="destination">Viewport point to move the cursor to [0-1][0-1].</param>
        /// <param name="borderTime">Time to move from one screen border to opposite border in seconds.</param>
        public static void MoveTo(Vector2 destination, float borderTime, float deltaTime)
        {
            if (borderTime < 0) borderTime = -borderTime;

            Vector2 toDestination = destination - pos;
            pos += toDestination * (deltaTime / borderTime);
            ClampPosition();
        }

        /// <summary>Instantly move the cursor by a pixel amount.</summary>
        /// <param name="movement">Movement vector in pixels.</param>
        /// <param name="screenDimensions">Pixel dimensions of the screen.</param>
        public static void FixedMove(Vector2 movement, Vector2 screenDimensions)
        {
            pos.x += movement.x / screenDimensions.x;
            pos.y += movement.y / screenDimensions.y;
            ClampPosition();
        }

        /// <summary>Converts viewport position to screenspace with specified width and height.</summary>
        public static Vector2 ToScreenSpace(float screenWidth, float screenHeight)
        {
            return new Vector2(screenWidth * pos.x, screenHeight * pos.y);
        }

        /// <returns>The cursor position, mapped -1 to 1.</returns>
        public static Vector2 ToNegativeOneToOne()
        {
            return (pos * 2) - Vector2.one;
        }
    }
}