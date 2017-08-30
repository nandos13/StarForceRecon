using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
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
    [Range(90.0f, 480.0f), SerializeField]  private float _movingTurningSpeed = 360.0f;
    [Range(90.0f, 360.0f), SerializeField]  private float _stationaryTurningSpeed = 180.0f;
    [Range(1.0f, 5.0f), SerializeField]     private float _gravityMultiplier = 2.0f;

    #endregion

    #region Tracker Variables
    
    private RaycastHit _hit;
    private Ray _ray;

    private bool _grounded = false;
    private Vector3 _groundNormal;
    private bool _crouching = false;

    private float _turn = 0.0f;
    private float _forward = 0.0f;

    #endregion


    private void Awake()
    {
        // Get references
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _col = GetComponent<CapsuleCollider>();

        // Store default values
        _colHeight = _col.height;
        _colCenter = _col.center;
        _originalGroudCheckDist = _groundCheckDist;

        // Constrain rigidbody
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void Move(Vector3 direction, bool crouch = false)
    {
        if (direction.magnitude <= 0)
            return;
        if (direction.magnitude != 1.0f)
            direction.Normalize();

        // Convert move vector into local space
        direction = transform.InverseTransformDirection(direction);

        // Check grounded status
        DoGroundCheck();

        // Project movement vector onto ground plane to prevent bunnyhopping off inclines, etc.
        direction = Vector3.ProjectOnPlane(direction, _groundNormal);

        _turn = Mathf.Atan2(direction.x, direction.z);
        _forward = direction.z;

        // Apply extra turning to help the character turn faster
        float turnSpeed = Mathf.Lerp(_stationaryTurningSpeed, _movingTurningSpeed, _forward);
        transform.Rotate(0, _turn * turnSpeed * Time.deltaTime, 0);

        // Add extra gravity if airborne
        if (!_grounded)
            ApplyGravity();

        ScaleCapsuleForCrouching(crouch);
        PreventStandingInLowHeadroom();

        UpdateAnimator(direction);
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
        _ray.origin = transform.position + (Vector3.up * 0.05f);
        _ray.direction = Vector3.down;

        if (Physics.Raycast(_ray, out _hit, _groundCheckDist))
        {
            // Grounded

            _grounded = true;
            _groundNormal = _hit.normal;
            _animator.applyRootMotion = true;
        }
        else
        {
            // Not grounded

            _grounded = false;
            _groundNormal = Vector3.up;
            _animator.applyRootMotion = false;
        }
    }
}
