using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleOnGunFired : MonoBehaviour
{
    [SerializeField]    private Gun _gun = null;
    [SerializeField]    private ParticleSystem _ps = null;
    [Range(1, 100), SerializeField] private int _count = 1;

    /// <summary>Wrapper class to store data passed to coroutine.</summary>
    private struct EmitDelayWrapper
    {
        public Vector3 origin;
        public Vector3 direction;
        public float delay;

        public EmitDelayWrapper(Vector3 orig, Vector3 dir, float t)
        {
            origin = orig;
            direction = dir;
            delay = t;
        }
    }

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
            float delay = Random.Range(0.0f, 0.07f);
            if (delay < 0.001f)
                Emit(origin, direction);
            else
            {
                EmitDelayWrapper w = new EmitDelayWrapper(origin, direction, delay);
                StartCoroutine("EmitAfterDelay", w);
            }
        }
        else
            Debug.Log("No ParticleSystem");
    }

    private IEnumerator EmitAfterDelay(EmitDelayWrapper w)
    {
        if (w.delay > 0)
            yield return new WaitForSeconds(w.delay);
        Emit(w.origin, w.direction);
    }

    private void Emit(Vector3 origin, Vector3 direction)
    {
        ParticleSystem.EmitParams p = new ParticleSystem.EmitParams();
        p.velocity = direction * _ps.main.startSpeedMultiplier;
        _ps.Emit(p, _count);
    }
}
