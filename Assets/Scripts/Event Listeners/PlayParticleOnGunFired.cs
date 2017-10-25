using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleOnGunFired : MonoBehaviour
{
    [SerializeField]    private Gun gun = null;
    [SerializeField]    private new ParticleSystem particleSystem = null;
    [Range(1, 100), SerializeField] private int _count = 1;

    private List<ShotInfoWrapper> pendingShotDirections = new List<ShotInfoWrapper>();

    private struct ShotInfoWrapper
    {
        public Vector3 origin;
        public Vector3 direction;

        public ShotInfoWrapper(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
        }
    }

    private void Awake()
    {
        if (!gun) gun = GetComponentInParent<Gun>();
        if (gun) gun.OnGunShotFired += _gun_OnShotFired;

        if (!particleSystem) particleSystem = GetComponentInChildren<ParticleSystem>();
        if (!particleSystem) throw new System.Exception("No Particle System attached.");
    }

    private void Update()
    {
        if (particleSystem && pendingShotDirections.Count > 0)
        {
            ShotInfoWrapper shot = pendingShotDirections[0];
            Emit(shot.origin, shot.direction);
            pendingShotDirections.RemoveAt(0);
        }
    }

    private void _gun_OnShotFired(Gun sender, Vector3 origin, Vector3 direction, Transform hitTransform)
    {
        pendingShotDirections.Add(new ShotInfoWrapper(origin, direction));
    }

    private void Emit(Vector3 origin, Vector3 direction)
    {
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
        emitParams.velocity = direction * particleSystem.main.startSpeedMultiplier;
        particleSystem.Emit(emitParams, _count);
    }
}
