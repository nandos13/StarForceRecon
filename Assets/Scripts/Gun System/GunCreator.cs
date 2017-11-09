using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class GunCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject gunPrefab = null;

    [SerializeField]
    private Transform rightHand = null;

    [SerializeField]
    private Vector3 handRotation = Vector3.zero;

    private void Awake()
    {
        if (gunPrefab != null && rightHand != null)
        {
            AimIK aimIK = GetComponent<AimIK>();

            GameObject newGun = Instantiate<GameObject>(gunPrefab);
            newGun.transform.parent = rightHand;
            GunHandPlacementOffset offsetter = newGun.GetComponentInChildren<GunHandPlacementOffset>();
            if (offsetter == null)
            {
                Destroy(newGun);
                throw new System.Exception("GunPrefab does not contain a GunHandPlacementOffset script! Fix this please.");
            }

            newGun.transform.forward = Quaternion.Euler(handRotation) * rightHand.forward;
            newGun.transform.position = rightHand.position - (offsetter.transform.rotation * offsetter.offset);
            Destroy(offsetter);

            Gun gunScript = newGun.GetComponentInChildren<Gun>();
            if (gunScript == null)
            {
                Destroy(newGun);
                throw new System.Exception("GunPrefab does not contain a Gun script! fix this please.");
            }

            StarForceRecon.Equipment equipment = GetComponentInChildren<StarForceRecon.Equipment>();
            if (equipment != null)
                equipment.SetSlot(0, gunScript);

            StarForceRecon.AimHandler aimHandler = GetComponentInChildren<StarForceRecon.AimHandler>();
            if (aimHandler != null)
                aimHandler.GunOrigin = gunScript.Origin;

            if (aimIK != null)
                aimIK.solver.transform = gunScript.Origin;
        }
        else
            throw new System.Exception("GunCreator must have a Gun Prefab and Hand Transform assigned.");
    }

    private void OnDrawGizmosSelected()
    {
        if (rightHand != null)
        {
            Quaternion rotation = Quaternion.Euler(handRotation);

            Vector3 fwd = (rotation * rightHand.forward).normalized;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(rightHand.position, fwd * 0.2f);
        }
    }
}
