using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThirdPersonController))]
public class PlayerMovement : MonoBehaviour
{

    ThirdPersonController _tpc = null;

    void Awake()
    {
        _tpc = GetComponent<ThirdPersonController>();
    }

    void FixedUpdate()
    {
        // Get input
        float horz = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");

        Vector3 move = Vector3.zero;
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 forward = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1)).normalized;
            move = vert * forward + horz * cam.transform.right;

            _tpc.Move(move);
        }
    }

    private void OnDisable()
    {
        _tpc.StopMovement();
    }
}
