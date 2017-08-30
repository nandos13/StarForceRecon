using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class DoorOpen : MonoBehaviour
{

    private bool GuiShow = false;
    private bool isOpen = false;

    
    public GameObject door;
    int rayLength = 10;
    private Vector3 fwd;
    private bool guiShow;

    void Update()
    {
        
        fwd = transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, fwd, out hit, rayLength))
        {
            if (hit.collider.gameObject.tag == "Door")
            {
                guiShow = true;
                if (Input.GetKeyDown("e") && isOpen == false)
                {
                    door.GetComponent<Animation>().Play("DoorOpen");
                    isOpen = true;
                    guiShow = false;
                }
                else if(Input.GetKeyDown("e") && isOpen == true)
                {
                    door.GetComponent<Animation>().Play("DoorClose");
                    isOpen = false;
                    guiShow = false;
                }
            }
        }

        else
        {
            guiShow = false;
        }
    }

    void OnGUI()
    {
        if (guiShow == true && isOpen == false)
        {
            GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 100, 25), "Use Door");
        }
    }
}