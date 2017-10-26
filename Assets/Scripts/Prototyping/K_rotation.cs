using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_rotation : MonoBehaviour {

    public float _speed;

	// Use this for initialization
	void FixedUpdate () {
        gameObject.transform.Rotate(Vector3.up * Time.deltaTime * _speed);
    }
}
