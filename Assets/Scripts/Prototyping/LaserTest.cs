using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarForceRecon;

[RequireComponent(typeof(LineRenderer))]
public class LaserTest : MonoBehaviour
{

    public Transform _origin;
    private NewPlayerController _aimScript;
    public LineRenderer _line;
    
	void Start ()
    {
        _aimScript = GetComponent<NewPlayerController>();

        if (!_line)
            _line = GetComponent<LineRenderer>();

        _line.textureMode = LineTextureMode.Tile;
    }
	
	void Update ()
    {
        if (_aimScript && _line && _origin)
        {
            Vector3 endPoint = _aimScript.aimPoint;
            Vector3[] positions = { _origin.position, endPoint};
            _line.SetPositions(positions);


            if (_aimScript.aiming)
                _line.enabled = true;
            else
                _line.enabled = false;
        }

        if (_line)
        {
            Vector2 laserOffset = _line.material.mainTextureOffset;
            laserOffset.x += Time.deltaTime * 0.1f;
            _line.material.mainTextureOffset = laserOffset;
        }
	}

    void OnDisable()
    {
        _line.enabled = false;
    }
}
