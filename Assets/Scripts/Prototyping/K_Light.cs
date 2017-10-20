using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_Light : MonoBehaviour {


    public GameObject _light;

    BoxCollider _box;

    void Awake()
    {
        _box = GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            Invoke("SwitchOn", 0.6f);
        }
    }

    void OnTriggerExit(Collider col)
    {
        bool playersInside = false;
        Collider[] cols = Physics.OverlapBox(_box.bounds.center, _box.size / 2, _box.transform.rotation);
        foreach (Collider c in cols)
        {
            if (c.tag == "Player")
            {
                playersInside = true;
                break;
            }
        }

        if(playersInside == false)
        {
            Invoke("SwitchOff", 1.0f);
        }
    }

    void SwitchOn()
    {
        _light.SetActive(true);
    }

    void SwitchOff()
    {
        _light.SetActive(false);
    }
}
