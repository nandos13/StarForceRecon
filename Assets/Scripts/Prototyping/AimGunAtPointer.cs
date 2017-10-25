using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarForceRecon;

public class AimGunAtPointer : MonoBehaviour
{
    private AimHandler aimer = null;

	void Start ()
    {

        aimer = GetComponentInParent<AimHandler>();
        if (!aimer)
            throw new System.MissingFieldException("No aimHandler script :(");
	}
	
	void Update ()
    {
        transform.LookAt(aimer.aimPoint, Vector3.up);
	}
}
