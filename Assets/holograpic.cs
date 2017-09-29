using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class holograpic : MonoBehaviour {

    public float speed;
    public float rotTime;

    public GameObject one, two;

    void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime);

        if (transform.forward.z < 0)
        {
            one.SetActive(false);
            two.SetActive(true);
        }
        else 
        {
            one.SetActive(true);
            two.SetActive(false);
        }
    }
}
