using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThirdPersonController))]
public class PlayerMovement : MonoBehaviour
{

    private ThirdPersonController _tpc = null;
    private bool _rollPressed;

    void Awake()
    {
        _tpc = GetComponent<ThirdPersonController>();
    }

    void FixedUpdate()
    {
        // Get directional input
        float horz = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");

        // Get rolling input, only true on initial key down
        bool roll = false;
        if (Input.GetAxisRaw("Roll") != 0)
        {
            if (!_rollPressed)
            {
                _rollPressed = true;
                roll = true;
            }
        }
        else
            _rollPressed = false;

        Vector3 move = Vector3.zero;
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 forward = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1)).normalized;
            move = vert * forward + horz * cam.transform.right;

            // TODO: Get input for crouch
            _tpc.Move(move, roll);
        }
    }

    private void OnDisable()
    {
        _tpc.StopMovement();
    }
}
