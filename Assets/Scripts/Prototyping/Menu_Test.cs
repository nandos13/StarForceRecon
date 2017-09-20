using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Test : MonoBehaviour {

    Animator _animator;
    bool settingMenu = false;
    bool pressed = false;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void AnyKey()
    {
        if(pressed == false)
        {
            
        }
    }

}
