using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarForceRecon;

[RequireComponent(typeof(LineRenderer))]
public class LaserTest : MonoBehaviour
{

    public Transform _origin;
    private AimHandler aim;
    private PlayerController player;
    private LineRenderer line;
    
	void Start ()
    {
        aim = GetComponent<AimHandler>();
        player = GetComponent<PlayerController>();
        line = GetComponent<LineRenderer>();

        line.textureMode = LineTextureMode.Tile;
    }
	
	void Update ()
    {
        if (aim && line && _origin)
        {
            Vector3 endPoint = aim.AimPoint;
            Vector3[] positions = { _origin.position, endPoint};
            line.SetPositions(positions);

            line.enabled = player.aiming;
        }

        if (line)
        {
            Vector2 laserOffset = line.material.mainTextureOffset;
            laserOffset.x += Time.deltaTime * 0.1f;
            line.material.mainTextureOffset = laserOffset;
        }
	}

    void OnDisable()
    {
        line.enabled = false;
    }
}
