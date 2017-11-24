using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using JakePerry;

namespace StarForceRecon
{
    [RequireComponent(typeof(ThirdPersonController)), 
        RequireComponent(typeof(AimHandler)),
        RequireComponent(typeof(Equipment)),
        DisallowMultipleComponent]
    public class PlayerController : MonoBehaviour, 
                                    GameController<SFRInputSet>.ITarget, 
                                    SquadManager.IControllable,
                                    Interaction.IInteractor
    {
        #region Variables

        #region Input Controls

        private const string controllerID = "PlayerController";

        private GameController<SFRInputSet> _keyboardController = null;
        private GameController<SFRInputSet> _gamepadController = null;
        private int _lastAimMouseTime = -1;
        private int _lastAimJoystickTime = -1;
        private int _validInputReceiveTime = -1;

        private Vector2 _moveDirection = Vector2.zero;
        private bool _rollKeyPressed = false;

        #endregion

        #region Inspector accessible

        [Header("Aiming")]

        [Tooltip("When aiming at a point within this radius from the character, the character will be locked to melee attacking only.")]
        [SerializeField, Range(1.0f, 3.0f)]
        private float closeAimRadius = 0.5f;

        [Tooltip("Time in seconds for cursor to fade out when the character is not aiming.")]
        [SerializeField, Range(0.2f, 2.0f)]
        private float cursorFadeTime = 1.0f;

        [Tooltip("Minimum time between melee attacks.")]
        [SerializeField, Range(0.0f, 2.0f)]
        private float meleeCooldown = 0.8f;

        [Tooltip("Melee attacks will affect any enemies with their transform position inside this box.")]
        [SerializeField]
        private BoxCollider meleeArea;

        [SerializeField, Range(0.0f, 10.0f)]
        private float meleeDamage = 3.0f;

        [Header("Character Switching")]

        [Tooltip("A list of AI scripts which will be enabled when the character is not being controlled by the player")]
        [SerializeField]
        private List<Behaviour> aiScripts;
        [Tooltip("A list of Player scripts which will be enabled when the character is being controlled by the player")]
        [SerializeField]
        private List<Behaviour> playerScripts;

        [Tooltip("Delay in seconds before re-enabling Controller scripts.")]
        [SerializeField, Range(0.2f, 1.0f)]
        private float selectionDelay = 0.2f;

        [SerializeField]
        private Color cursorColor = Color.red;

        [SerializeField, Range(0.1f, 1.0f)]
        private float colorFadeTime = 0.25f;

        #endregion

        #region Private State Trackers & References

        private ThirdPersonController _tpc = null;
        private AimHandler _aimHandler = null;
        private Equipment _equipment = null;
        private Animator animator = null;

        private static readonly Vector2 CENTER_VIEWPORT = new Vector2(0.5f, 0.5f);
        private static readonly Vector3 HORIZON_SCALE_VECTOR = new Vector3(1, 0, 1);

        public bool aiming { get; private set; }
        private bool meleeLocked = false;

        private bool secondaryIsEquipped = false;

        private static Canvas cursorCanvas = null;
        private static UnityEngine.UI.Image cursorSprite = null;

        private Interaction.IInteractable closestInteractable = null;

        private Coroutine cursorColorFader = null;

        #endregion

        #endregion

        #region Functionality

        #region Unity Lifetime Functions

        private void Awake()
        {
            // Initialize aimer canvas
            if (cursorCanvas == null)
                cursorCanvas = CreateCursor(out cursorSprite);
            EnableCursor();

            // Initialize Input
            _keyboardController = ControllerManager<KeyboardMouseController<SFRInputSet>, SFRInputSet>.GetController(controllerID, this);
            _gamepadController = ControllerManager<DualStickController<SFRInputSet>, SFRInputSet>.GetController(controllerID, this);

            // Get component references
            _tpc = GetComponent<ThirdPersonController>();
            _aimHandler = GetComponent<AimHandler>();
            _equipment = GetComponent<Equipment>();
            animator = GetComponentInChildren<Animator>();

            if (_tpc == null)
                throw new System.MissingFieldException("No Third Person Controller component found.");
            if (_aimHandler == null)
                throw new System.MissingFieldException("No Aim Handler component found.");
            if (_equipment == null)
                throw new System.MissingFieldException("No Equipment component found.");

            _tpc.OnRollStart += _tpc_OnRollStart;
            _tpc.OnRollEnd += _tpc_OnRollEnd;
        }

        private void _tpc_OnRollEnd()
        {
            _aimHandler.IKState = true;
        }

        private void _tpc_OnRollStart()
        {
            _aimHandler.IKState = false;
        }

        private void Start()
        {
            // Register to the Squad Manager
            SquadManager.AddSquadMember(this);
        }

        private void Update()
        {
            UpdateOnScreenCursor();
            if (_tpc.isRolling) return;

            // Find aim point, limited to minimum radius
            _aimHandler.HandlePlayerAiming(StarForceRecon.Cursor.position);
            float horizontalDistance = Vector3.Distance(transform.position,
                new Vector3(_aimHandler.AimPoint.x, 0, _aimHandler.AimPoint.z));
            aiming = (horizontalDistance >= closeAimRadius);

            // Check for interactables around the character
            CheckClosestInteractable();
        }

        private void FixedUpdate()
        {
            MovePlayerCharacter();
        }

        private void OnDisable()
        {
            _tpc.StopMovement();
        }

        private void OnDestroy()
        {
            ControllerManager<KeyboardMouseController<SFRInputSet>, SFRInputSet>.RemoveTarget(controllerID, this);
            ControllerManager<DualStickController<SFRInputSet>, SFRInputSet>.RemoveTarget(controllerID, this);

            if (OnControlTargetDestroyed != null)
                OnControlTargetDestroyed.Invoke(this);
        }

        #endregion

        #region Visual Cursor

        public static void DisableCursor()
        {
            if (cursorCanvas == null)
                CreateCursor(out cursorSprite);

            cursorCanvas.enabled = false;
        }

        public static void EnableCursor()
        {
            if (cursorCanvas == null)
                CreateCursor(out cursorSprite);

            cursorCanvas.enabled = true;
        }

        private static Canvas CreateCursor(out UnityEngine.UI.Image cursorSprite)
        {
            GameObject canvasObject = new GameObject("Aiming Cursor");
            canvasObject.hideFlags = HideFlags.HideAndDontSave;
            GameObject.DontDestroyOnLoad(canvasObject);
            canvasObject.AddComponent<PlayerController.CursorDestroyBehaviour>();

            GameObject cursorObject = new GameObject("Cursor sprite");
            cursorObject.transform.parent = canvasObject.transform;

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            UnityEngine.UI.CanvasScaler scaler = canvasObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            UnityEngine.UI.Image image = cursorObject.AddComponent<UnityEngine.UI.Image>();
            image.rectTransform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            image.sprite = Resources.Load<Sprite>("Sprites/AimCursor");

            cursorSprite = image;
            return canvas;
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

        #endregion

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
        
        /// <summary>Makes the character aim at a certain point based on input.</summary>
        private void AimPlayerCharacter(Vector2 aimInput, GameController<SFRInputSet>.ControlType type)
        {
            bool useJoystickAim = false;

            if (type == GameController<SFRInputSet>.ControlType.Gamepad)
            {
                // Use joystick aiming if controller has been used more recently than mouse
                useJoystickAim = (_lastAimMouseTime + 1) < _lastAimJoystickTime;

                if (aimInput.x != 0 || aimInput.y != 0) _lastAimJoystickTime = Time.frameCount;
            }
            else
                if (aimInput.x != 0 || aimInput.y != 0) _lastAimMouseTime = Time.frameCount;
            
            ModifyCursorPosition(aimInput, useJoystickAim);
        }

        private void HandleActionInput(SFRInputSet inputSet)
        {
            if (inputSet.GetActionByName("switchForward").WasPressed)
            {
                SquadManager.Switch(true);
                return;
            }

            if (inputSet.GetActionByName("switchBack").WasPressed)
            {
                SquadManager.Switch(false);
                return;
            }
            
            if (inputSet.GetActionByName("roll").WasPressed)
                _rollKeyPressed = true;
            if (_rollKeyPressed || _tpc.isRolling) return; // Prevent other actions if rolling

            if (!meleeLocked && inputSet.GetActionByName("melee").WasPressed)
                StartMeleeAttack();
            if (meleeLocked) return; // Prevent other actions if doing a melee attack

            if (inputSet.GetActionByName("interact").WasPressed)
            {
                if (closestInteractable != null)
                {
                    Interaction.StartInteraction(this, closestInteractable);
                    return;
                }
            }

            if (inputSet.GetActionByName("switchWeapon").WasPressed)
            {
                _equipment.PrimarySecondarySwap();
                // TODO: Disable firing until swap animation is done
                return;
            }

            if (inputSet.GetActionByName("fire").IsPressed)
                _equipment.Use(Equipment.Slot.Primary);

            // TODO: Other equipment
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

        /// <summary>Records input to be applied on the next FixedUpdate call.
        /// Once input has been received, input is locked until the next FixedUpdate.</summary>
        private void HandleMovementInput(Vector2 moveInput)
        {
            if (_validInputReceiveTime < 0)
            {
                _moveDirection.x = moveInput.x;
                _moveDirection.y = moveInput.y;

                _validInputReceiveTime = Time.frameCount;
            }
            else
            {
                // Take the largest of movement values
                if (moveInput.magnitude > _moveDirection.magnitude)
                {
                    _moveDirection.x += moveInput.x;
                    _moveDirection.y += moveInput.y;
                }
            }
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
            if (!meleeLocked && meleeArea != null)
            {
                // Do animation
                animator.SetTrigger("Melee");

                // Damage all within melee area
                Collider[] collidersInMeleeArea = Physics.OverlapBox(meleeArea.center, meleeArea.bounds.extents);
                foreach (var col in collidersInMeleeArea)
                {
                    Health health = col.GetComponentInChildren<Health>();

                    DamageLayer.Mask mask = new DamageLayer.Mask();
                    mask.SetLayerState(DamageLayer.Utils.NameToLayer("Enemy"), true);
                    mask.SetLayerState(DamageLayer.Utils.NameToLayer("Player"), false);

                    DamageLayer.Modifier modifier = new DamageLayer.Modifier();

                    DamageData damage = new DamageData(this, meleeDamage, mask, modifier);

                    health.ApplyDamage(damage);
                }

                meleeLocked = true;
                StartCoroutine(meleeLockCooldown(meleeCooldown));
            }
        }

        private IEnumerator meleeLockCooldown(float lockTime)
        {
            yield return new WaitForSeconds(lockTime);
            meleeLocked = false;
        }

        #region Interaction IInteractor Methods

        private void CheckClosestInteractable()
        {
            const float interactableRadius = 10.0f;

            Collider[] colliders = Physics.OverlapSphere(transform.position, interactableRadius);
            KeyValuePair<Transform, Interaction.IInteractable>[] interactables = colliders
                .Select(col => new KeyValuePair<Transform, Interaction.IInteractable>(col.transform, 
                                col.transform.GetComponentInParent<Interaction.IInteractable>()))
                .Where(i => i.Value != null)
                .Distinct()
                .ToArray();
            
            float minDistance = float.MaxValue;
            Interaction.IInteractable best = null;
            foreach (var pair in interactables)
            {
                float distance = Vector3.Distance(new Vector3(pair.Key.position.x, transform.position.y, pair.Key.position.z), transform.position);
                if (distance < minDistance)
                {
                    best = pair.Value;
                    minDistance = distance;
                }
            }

            closestInteractable = best;
        }

        private bool lockedViaInteraction = false;
        void Interaction.IInteractor.OnStartInteraction(Interaction.InteractionInfo info)
        {
            if (info.Target.ForceCharacterSwap)
            {
                lockedViaInteraction = true;
                squadControllable = false;
                SquadManager.Switch();

                CancelInvoke("SelectCharacter");

                foreach (Behaviour behaviour in aiScripts)
                    behaviour.enabled = false;

                foreach (Behaviour behaviour in playerScripts)
                    behaviour.enabled = false;
            }

            if (info.Target.Type == Interaction.InteractionType.HiveDestruct)
            {
                if (animator != null)
                    animator.SetTrigger("GrenadeInteraction");
            }
        }

        void Interaction.IInteractor.OnCompleteInteraction()
        {
            if (lockedViaInteraction)
            {
                lockedViaInteraction = false;
                squadControllable = true;

                foreach (Behaviour behaviour in aiScripts)
                    behaviour.enabled = true;
            }
        }

        #endregion

        #region GameController ITarget Methods

        void GameController<SFRInputSet>.ITarget.ReceiveControllerInput(SFRInputSet inputSet, 
            GameController<SFRInputSet>.ControlType controllerType)
        {
            if (isActiveAndEnabled && Time.timeScale > 0)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;

                HandleMovementInput(inputSet.GetDualAxisByName("movement").Value);
                
                AimPlayerCharacter(inputSet.GetDualAxisByName("looking"), controllerType);

                HandleActionInput(inputSet);
            }
        }

        #endregion

        private bool squadControllable = true;
        #region SquadManager IControllable Methods

        private void SelectCharacter()
        {
            foreach (Behaviour behaviour in aiScripts)
                behaviour.enabled = false;

            foreach (Behaviour behaviour in playerScripts)
                behaviour.enabled = true;

            // Enable this script
            this.enabled = true;

            // Change cursor colour to this character's colour
            if (cursorColorFader != null)
                StopCoroutine(cursorColorFader);
            cursorColorFader = StartCoroutine(CursorColorFade(cursorSprite.color, cursorColor, 0.15f));
        }

        private IEnumerator CursorColorFade(Color start, Color end, float time)
        {
            float elapsed = 0;
            while (elapsed < time)
            {
                elapsed += Time.deltaTime;
                float normalized = Mathf.Clamp01(elapsed / time);

                Color newColor = new Color();
                newColor.r = Mathf.Lerp(start.r, end.r, normalized);
                newColor.g = Mathf.Lerp(start.g, end.g, normalized);
                newColor.b = Mathf.Lerp(start.b, end.b, normalized);
                newColor.a = Mathf.Lerp(start.a, end.a, normalized);

                cursorSprite.color = newColor;

                yield return null;
            }
        }

        event SquadManager.ControllableDestroy OnControlTargetDestroyed;

        event SquadManager.ControllableDestroy SquadManager.IControllable.OnControlTargetDestroyed
        {
            add { OnControlTargetDestroyed += value; }
            remove { OnControlTargetDestroyed -= value; }
        }

        void SquadManager.IControllable.OnSwitchAway()
        {
            CancelInvoke("SelectCharacter");

            foreach (Behaviour behaviour in aiScripts)
                behaviour.enabled = true;

            foreach (Behaviour behaviour in playerScripts)
                behaviour.enabled = false;

            // Disable this script
            this.enabled = false;
        }

        void SquadManager.IControllable.OnSwitchTo()
        {
            Invoke("SelectCharacter", selectionDelay);
        }
        
        bool SquadManager.IControllable.Controllable
        { get { return squadControllable; } }

        Transform SquadManager.IControllable.transform
        { get { return transform; } }

        #endregion
        
        #endregion

        #region Cursor Destroy Behaviour

        /// <summary>Private behaviour added to the cursor canvas to handle destroy without errors throwing.</summary>
        private class CursorDestroyBehaviour : MonoBehaviour
        {
            private void OnApplicationQuit()
            {
                if (PlayerController.cursorSprite != null)
                {
                    GameObject imageObject = PlayerController.cursorSprite.gameObject;
                    DestroyImmediate(PlayerController.cursorSprite);
                    Destroy(imageObject);
                }

                PlayerController.cursorSprite = null;
                PlayerController.cursorCanvas = null;
            }
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
