using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHandPlacementOffset : MonoBehaviour
{
    public Vector3 offset;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + offset, new Vector3(0.1f, 0.1f, 0.1f));
        Gizmos.DrawWireSphere(transform.position + offset, 0.01f);
    }
}
