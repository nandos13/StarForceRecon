using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarForceRecon;

public class GunShotCounter : MonoBehaviour
{
    private int shotCount = 0;
    public int display;

    private void Start()
    {
        Gun gun = GetComponentInChildren<Gun>();

        if (gun != null)
            gun.OnGunFired += OnGunShot;
        else
            throw new System.Exception("No gun script found, cannot record shots.");
    }

    private void OnGunShot(Gun sender)
    {
        shotCount++;
        display = shotCount;
    }
}
