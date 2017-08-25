using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleOnGunFired : MonoBehaviour
{
    [SerializeField]    private Gun _gun = null;
    [SerializeField]    private ParticleSystem _ps = null;
    [Range(1, 100), SerializeField] private int _count = 1;

    private void Awake()
    {
        if (!_gun)
            _gun = GetComponentInParent<Gun>();

        if (_gun)
            _gun.OnGunShotFired += _gun_OnShotFired;

        if (!_ps)
            _ps = GetComponentInChildren<ParticleSystem>();

        if (!_ps)
            Debug.LogWarning("Warning: No particle system attached.", this);
    }

    private void _gun_OnShotFired(Gun sender, Vector3 origin, Vector3 direction)
    {
        if (_ps)
        {
            _ps.transform.LookAt(origin + direction);
            _ps.Emit(_count);
        }
        else
            Debug.Log("No ParticleSystem");
    }
}
