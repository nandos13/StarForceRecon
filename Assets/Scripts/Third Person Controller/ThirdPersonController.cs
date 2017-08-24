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

    public void Move(Vector3 direction)
    {
        if (direction.magnitude <= 0)
            return;
        if (direction.magnitude != 1.0f)
            direction.Normalize();

        // TODO
    }
}
