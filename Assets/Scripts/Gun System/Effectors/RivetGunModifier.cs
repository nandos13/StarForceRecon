using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RivetGunModifier : MonoBehaviour
{

    [SerializeField]    private Gun _gun;

    private void Awake()
    {
        if (!_gun)
            throw new System.MissingFieldException("No gun specified in RivetGunModifier script.");

        _gun.OnGunShotFired += gun_OnGunShotFired; ;
    }

    private void gun_OnGunShotFired(Gun sender, Vector3 origin, Vector3 direction, Transform hitTransform)
    {
        if (hitTransform != null)
        {
            IRivetable rivet = hitTransform.GetComponentInParent<IRivetable>();

            if (rivet != null)
                rivet.RemoveRivet();
        }
    }
}
