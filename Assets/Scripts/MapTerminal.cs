using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTerminal : MonoBehaviour
{

    public GameObject mapTerm;
    bool _paused;
    bool canShowMap = false;

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            canShowMap = true;
        }
            
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            canShowMap = false;
        }
            
    }

    void Update()
    {
        if (canShowMap)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (mapTerm.activeSelf)
                {
                    mapTerm.SetActive(false);
                    Time.timeScale = 1;
                }
                else
                {
                    mapTerm.SetActive(true);
                    Time.timeScale = 0;
                }
            }
        }
    }
}

//    void Update()
//    {
//        if (mapTerm == false && !_paused)
//        {
//            if (Input.GetKeyDown(KeyCode.F))
//            {
//                SetPauseState(true);
//            }
//        }

//        else if (mapTerm == true && _paused)
//        {
//            if (Input.GetKeyDown(KeyCode.F))
//            {
//                SetPauseState(false);
//            }
//        }
//    }

//    private void SetPauseState(bool state)
//    {
//        mapTerm.SetActive(!state);
//        Time.timeScale = state ? 0.0f : 1.0f;
//        Cursor.visible = state;
//        if (state)
//            Cursor.lockState = CursorLockMode.None;
//        mapTerm.SetActive(state);
//        _paused = state;
//    }
//}
