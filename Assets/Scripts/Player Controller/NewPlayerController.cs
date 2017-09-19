using UnityEngine;
using JakePerry;

namespace StarForceRecon
{
    [RequireComponent(typeof(ThirdPersonController)), DisallowMultipleComponent]
    public class NewPlayerController : MonoBehaviour, GameController.ITarget
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

        [Tooltip("When aiming, the character will turn towards aimer when the angle from forward exceeds this value.")]
        [SerializeField, Range(20.0f, 80.0f)]
        private float maxHipSwivel = 50.0f;

        [Tooltip("Character's turning speed in rotations/second.")]
        [SerializeField, Range(1.0f, 2.0f)]
        private float turningSpeed = 1.0f;

        #endregion

        private ThirdPersonController _tpc = null;

        private static readonly Vector2 CENTER_VIEWPORT = new Vector2(0.5f, 0.5f);
        private static readonly Vector3 SCALE_VECTOR = new Vector3(1, 0, 1);    // Used to get the camera's horizon forward, declared here to prevent memory allocation each frame

        #endregion

        #region Functionality

        private void Awake()
        {
            // Initialize Input
            _keyboardController = new KeyboardMouseController(this);
            _gamepadController = new DualStickController(this);

            _tpc = GetComponent<ThirdPersonController>();
        }

        private void Update()
        {
            RotateToFaceAimCheck(StarForceRecon.Cursor.position, maxHipSwivel);
        }

        private void FixedUpdate()
        {
            MovePlayerCharacter();
        }

        /// <summary>Moves the cursor on the screen based on given input.</summary>
        /// <param name="aimInput">The aiming input.</param>
        /// <param name="useJoystickAim">Should this input be treated as a joystick?</param>
        private void ModifyCursorPosition(Vector2 aimInput, bool useJoystickAim)
        {
            const float CONTROLLER_SPEED = 0.03f;   // (Lower is faster)    // TODO: Make a sensitivity slider
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
        private bool CharacterHorizonIntersect(Vector2 viewportCursorPosition, out Vector3 intersect)
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
                Vector3 forward = Vector3.Scale(cam.transform.forward, SCALE_VECTOR).normalized;
                move = _moveDirection.y * forward + _moveDirection.x * cam.transform.right;

                // TODO: Get input for crouch
                _tpc.Move(move, _rollKeyPressed);
            }

            // Reset input tracking to allow extra input to be received
            _validInputReceiveTime = -1;
            _rollKeyPressed = false;
        }

        #region GameController ITarget Methods

        void GameController.ITarget.ReceiveActionInput(GameController.ActionState actionState)
        {
            if (isActiveAndEnabled)
            {
                if (actionState.Action1.WasPressed) // If ROLL key was pressed this frame
                    _rollKeyPressed = true;
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
                float total = moveInput.x + moveInput.y;
                float currentTotal = _moveDirection.x + _moveDirection.y;

                if (total > currentTotal)
                {
                    _moveDirection.x += moveInput.x;
                    _moveDirection.y += moveInput.y;
                }
            }
        }

        #endregion

        private void OnDisable()
        {
            _tpc.StopMovement();
        }

        /* ***************     TESTING, DELETE SOON     *************** */
        private static GameObject cursorObjectTESTING;
        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                if (cursorObjectTESTING == null)
                {
                    cursorObjectTESTING = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    cursorObjectTESTING.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                }

                // Draw cursor
                Ray ray = Camera.main.ViewportPointToRay(StarForceRecon.Cursor.position);
                Vector3 cursorPosition = ray.GetPoint(3.0f);
                cursorObjectTESTING.transform.position = cursorPosition;
            }
        }
        /* ***************     END TESTING     *************** */

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
        
        /// <returns>The cursor position, mapped -1 to 1.</returns>
        public static Vector2 ToNegativeOneToOne()
        {
            return (pos * 2) - Vector2.one;
        }
    }
}
