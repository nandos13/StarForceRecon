using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarForceRecon;

[RequireComponent(typeof(LineRenderer))]
public class LaserTest : MonoBehaviour
{
    [SerializeField]
    private Transform origin;
    private AimHandler aim;
    private PlayerController player;
    private LineRenderer line;

    private RaycastHit[] hitCache = new RaycastHit[16];
    
	void Start ()
    {
        if (origin == null)
            throw new System.MissingFieldException("Origin cannot be null");

        aim = GetComponentInParent<AimHandler>();
        player = GetComponentInParent<PlayerController>();
        line = GetComponent<LineRenderer>();
    }
	
	void Update ()
    {
        if (line && origin)
        {
            RaycastHit endHit;
            Vector3 endPoint;
            Ray ray = new Ray(origin.position, origin.forward);
            if (RaycastingHelper.GetFirstValidHitNonAlloc(out endHit, ref hitCache, ray, 100.0f, aim.AimLayers, aim.AimTags))
                endPoint = endHit.point;
            else
                endPoint = ray.GetPoint(100.0f);

            Vector3[] positions = { origin.position, endPoint };
            line.SetPositions(positions);

            line.enabled = player.aiming;
        }
	}

    void OnDisable()
    {
        line.enabled = false;
    }
}
