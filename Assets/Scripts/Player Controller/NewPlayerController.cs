using UnityEngine;
using JakePerry;

[RequireComponent(typeof(ThirdPersonController))]
public class NewPlayerController : MonoBehaviour, GameController.ITarget
{
    // TODO:

    #region Variables

    #region Input Controls

    private GameController _keyboardController = null;
    private GameController _gamepadController = null;
    private int _validInputReceiveTime = -1;

    private Vector2 _moveDirection = Vector2.zero;
    private bool _rollKeyPressed = false;

    private Vector2 _lookDirection = Vector2.zero;
    private Vector2 _lookDirectionAnalog = Vector2.zero;
    private bool _useAnalogAiming = false;
    private Vector2 _cursorPosition = Vector2.zero;
    
    #endregion

    private ThirdPersonController _tpc = null;

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
        AimPlayerCharacter();
    }

    private void FixedUpdate()
    {
        MovePlayerCharacter();
    }

    /// <summary>Makes the character aim at a certain point based on pre-recorded input.</summary>
    private void AimPlayerCharacter()
    {
        Vector2 finalCursorPosition;

        if (_useAnalogAiming)   // Should the aim input be treated as a normalized value?
            finalCursorPosition = _lookDirectionAnalog; // Use analog aiming, where -1 to 1 are screen bounds
        else
        {
            // Add aim input to cursor position
            _cursorPosition += _lookDirection;
            _cursorPosition.x = Mathf.Clamp(_cursorPosition.x, -1, 1);
            _cursorPosition.y = Mathf.Clamp(_cursorPosition.y, -1, 1);

            finalCursorPosition = _cursorPosition;
        }
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
        if (actionState.Action1.WasPressed) // If ROLL key was pressed this frame
            _rollKeyPressed = true;
    }

    void GameController.ITarget.ReceiveAimInput(float horizontal, float vertical)
    {
        _lookDirection.x = horizontal;
        _lookDirection.y = vertical;
    }

    void GameController.ITarget.ReceiveAnalogAimInput(float horizontal, float vertical)
    {
        if (horizontal > 0 || vertical > 0)
        {
            _useAnalogAiming = true;

            _lookDirectionAnalog.x = horizontal;
            _lookDirectionAnalog.y = vertical;
        }
        else
            _useAnalogAiming = false;
    }

    void GameController.ITarget.ReceiveMoveInput(float horizontal, float vertical)
    {
        // Once input has been received, input is locked until the next FixedUpdate call
        if (_validInputReceiveTime < 0)
        {
            _moveDirection.x = horizontal;
            _moveDirection.y = vertical;

            _validInputReceiveTime = Time.frameCount;
        }
        else
        {
            // Take the largest of movement values
            float total = horizontal + vertical;
            float currentTotal = _moveDirection.x + _moveDirection.y;

            if (total > currentTotal)
            {
                _moveDirection.x += horizontal;
                _moveDirection.y += vertical;
            }
        }
    }

    #endregion

    private void OnDisable()
    {
        _tpc.StopMovement();
    }

    #endregion
}
