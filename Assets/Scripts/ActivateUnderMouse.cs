using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateUnderMouse : MonoBehaviour
{
    
	void Start ()
    {
		
	}
	
	void Update ()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Bang a = hit.collider.gameObject.GetComponent<Bang>();
                if (!a)
                    a = hit.collider.gameObject.GetComponentInChildren<Bang>();
                if (!a)
                    a = hit.collider.gameObject.GetComponentInParent<Bang>();
                if (a)
                {
                    a.Execute();
                }
                Debug.Log("you hit " + hit.transform.name);
            }
        }
    }
}
