using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheadHUD : MonoBehaviour {

    // who the HUD is attached to
    public Transform target;
    private Camera cam;

	// Use this for initialization
	void Start () {
        cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = target.transform.position + Vector3.up;
        transform.forward = cam.transform.forward;
	}
}
