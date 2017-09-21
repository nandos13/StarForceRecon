using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarForceRecon;

public class AimGunAtPointer : MonoBehaviour
{
  
    private NewPlayerController _aim = null;

	void Start ()
    {

        _aim = GetComponentInParent<NewPlayerController>();
        if (!_aim)
            throw new System.MissingFieldException("No controller :(");
	}
	
	void Update ()
    {
        if (_aim)
        {
            if (_aim.aiming)
            {
                Vector3 point = _aim.aimPoint;
                transform.LookAt(point, Vector3.up);
            }
        }
	}
}
