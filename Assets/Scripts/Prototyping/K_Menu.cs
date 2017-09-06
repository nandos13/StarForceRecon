using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class K_Menu : MonoBehaviour {

    Animator _animator;

    bool settingMenu = false;
    bool pressed = false;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SettingsPress()
    {
        if (pressed == false)
        {
            if (settingMenu == false)
            {
                pressed = true;
                Invoke("PressReset", 1f);
                settingMenu = true;
                _animator.SetBool("SettingsMenu", true);
            }

            else if (settingMenu == true)
            {
                pressed = true;
                Invoke("PressReset", 1f);
                settingMenu = false;
                _animator.SetBool("SettingsMenu", false);
            }
        }
    }

    void PressReset()
    {
        pressed = false;
    }
       

}
