using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class ThirdPersonController : MonoBehaviour
{
    #region References

    private Rigidbody _rb = null;
    private Animator _animator = null;
    private CapsuleCollider _col = null;

    #endregion

    #region Default Values

    private float _originalGroudCheckDist = 0.0f;
    private float _colHeight = 0.0f;
    private Vector3 _colCenter = Vector3.zero;

    #endregion

    #region Inspector Variables

    [Range(0.05f, 1.5f), SerializeField]    private float _groundCheckDist = 0.1f;
    [Range(1.0f, 5.0f), SerializeField]     private float _gravityMultiplier = 2.0f;
    [Range(0.5f, 3.0f), SerializeField]     private float _moveSpeedMultiplier = 1.0f;
    [Range(0.5f, 3.0f), SerializeField]     private float _animSpeedMultiplier = 1.0f;

    // Rolling
    [SerializeField]    private bool _canRoll = false;
    [SerializeField]    private AnimationCurve _rollSpeed = new AnimationCurve(new Keyframe(0, 10), new Keyframe(1, 0));
    [Tooltip("Duration of rolling animation in seconds.")]
    [Range(0.1f, 3.0f), SerializeField]     private float _rollTime = 1.0f;

    [Tooltip("Damp time for direction change. Lower value will result in quicker changes in direction, but may result in choppy animation blends.")]
    [Range(0.01f, 0.1f), SerializeField]    private float _directionalBlendDamp = 0.1f;

    #endregion

    #region Tracker Variables

    private RaycastHit _hit = new RaycastHit();
    private Ray _ray = new Ray();

    private bool _grounded = false;
    private Vector3 _groundNormal;
    private bool _crouching = false;
    private bool _rolling = false;
    private Vector3 _rollDir;
    private float _rollTimeElapsed = 0.0f;

    private float _forward = 0.0f;
    private float _right = 0.0f;

    #endregion

    public bool isRolling { get { return _rolling; } }

    private void Awake()
    {
        // Get references
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _col = GetComponent<CapsuleCollider>();

        if (_animator == null)
            throw new System.MissingFieldException("Third Person Controller requires an Animator component on a descendant.");

        // Store default values
        _colHeight = _col.height;
        _colCenter = _col.center;
        _originalGroudCheckDist = _groundCheckDist;

        // Constrain components
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
        _animator.applyRootMotion = false;
    }

    void Update()
    {
        // Handle rolling movement
        if (_rolling)
        {
            // Rotate to face rolling direction
            Quaternion rotation = Quaternion.LookRotation(_rollDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 20f);

            // Get rolling velocity vector
            float normalizedRollTime = _rollTimeElapsed / _rollTime;
            Vector3 v = (_rollDir * _rollSpeed.Evaluate(normalizedRollTime));

            v.y = _rb.velocity.y;
            _rb.velocity = v;

            // Check for rolling end
            _rollTimeElapsed += Time.deltaTime;

            if (_rollTimeElapsed >= _rollTime)
                EndRoll();
        }
    }

    public void Move(Vector3 direction, bool roll = false, bool crouch = false)
    {
        if (direction.magnitude <= 0)
        {
            _animator.SetBool("IsWalking", false);
            return;
        }
        if (direction.magnitude != 1.0f)
            direction.Normalize();

        direction = transform.InverseTransformDirection(direction);

        // Check grounded status
        DoGroundCheck();

        // Project movement vector onto ground plane to prevent bunnyhopping off inclines, etc.
        direction = Vector3.ProjectOnPlane(direction, _groundNormal);

        _forward = direction.z;
        _right = direction.x;

        // Add extra gravity if airborne
        if (!_grounded)
            ApplyGravity();

        if (_grounded && Time.deltaTime > 0)
        {
            if (_canRoll)
            {
                // Should a new roll be started?
                if (!_rolling && roll)
                {
                    // Start a new roll
                    _rollTimeElapsed = 0.0f;
                    _rolling = true;
                    _rollDir = transform.TransformDirection(direction);
                }
            }
            else
                _rolling = false;

            // Do state-specific movement
            if (!_rolling)
            {
                // Do standard grounded movement

                Vector3 v = (direction * _moveSpeedMultiplier);
                v = transform.TransformDirection(v);
                
                v.y = _rb.velocity.y;
                _rb.velocity = v;
            }
        }

        ScaleCapsuleForCrouching(crouch);
        PreventStandingInLowHeadroom();

        UpdateAnimator(direction);
    }

    private void EndRoll()
    {
        // End roll
        _rolling = false;
        _animator.SetBool("Roll", false);

        // Stop roll momentum
        Vector3 v = Vector3.zero;
        v.y = _rb.velocity.y;
        _rb.velocity = v;
    }

    private void ScaleCapsuleForCrouching(bool crouch)
    {
        if (_grounded && crouch)
        {
            if (_crouching) return;
            _col.height = _col.height / 2.0f;
            _col.center = _col.center / 2.0f;
            _crouching = true;
        }
        else
        {
            _ray.origin = _rb.position + Vector3.up * _col.radius * 0.5f;
            _ray.direction = Vector3.up;

            float rayLength = _colHeight - _col.radius * 0.5f;

            if (Physics.SphereCast(_ray, _col.radius * 0.5f, rayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                _crouching = true;
                return;
            }

            _col.height = _colHeight;
            _col.center = _colCenter;
            _crouching = false;
        }
    }

    private void PreventStandingInLowHeadroom()
    {
        if (!_crouching)
        {
            _ray.origin = _rb.position + Vector3.up * _col.radius * 0.5f;
            _ray.direction = Vector3.up;

            float rayLength = _colHeight - _col.radius * 0.5f;

            if (Physics.SphereCast(_ray, _col.radius * 0.5f, rayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                _crouching = true;
        }
    }

    private void UpdateAnimator(Vector3 move)
    {
        // Update animator parameters
        _animator.SetBool("IsWalking", true);

        _animator.SetBool("Roll", _rolling);

        _animator.SetFloat("Forward", _forward, _directionalBlendDamp, Time.deltaTime);
        _animator.SetFloat("Right", _right, _directionalBlendDamp, Time.deltaTime);

        //_animator.SetBool("Crouch", _crouching);
        _animator.SetBool("Grounded", _grounded);

        if (_grounded && move.magnitude > 0)
            _animator.speed = _animSpeedMultiplier;
        else
            _animator.speed = 1.0f;
    }

    private void ApplyGravity()
    {
        // Apply extra gravity
        Vector3 extraGravity = (Physics.gravity * _gravityMultiplier) - Physics.gravity;
        _rb.AddForce(extraGravity);

        // Use tiny ground check distance if the character is moving upwards
        _groundCheckDist = (_rb.velocity.y < 0) ? _originalGroudCheckDist : 0.01f;
    }

    private void DoGroundCheck()
    {
        float radius = _col.radius * 0.8f;
        _ray.origin = transform.position + (Vector3.up * (0.05f + radius));
        _ray.direction = Vector3.down;
        
        if (Physics.SphereCast(_ray, radius, out _hit, _groundCheckDist))
        {
            // Grounded

            _grounded = true;
            _groundNormal = _hit.normal;
        }
        else
        {
            // Not grounded

            _grounded = false;
            _groundNormal = Vector3.up;
        }
    }

    public void StopMovement()
    {
        if (_animator.gameObject.activeSelf)
            _animator.SetBool("IsWalking", false);

        Vector3 newVelocity = Vector3.zero;
        newVelocity.y = _rb.velocity.y;
        _rb.velocity = newVelocity;
    }

    private void OnDrawGizmosSelected()
    {
        if (_animator)
        {
            if (_animator.gameObject.activeSelf)
            {
                if (_animator.GetBool("IsWalking"))
                {
                    Vector3 walkDirection = new Vector3(_animator.GetFloat("Right"), 0, _animator.GetFloat("Forward"));
                    walkDirection = transform.TransformDirection(walkDirection);
                    Gizmos.color = Color.blue;

                    Gizmos.DrawRay(transform.position, walkDirection);
                }
            }
        }
    }
}
