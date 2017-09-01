using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    Animator animator;

    Animator GetAnimator()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        return animator;
    }

    bool settingMenu = false;
    bool pressed = false;
        
    public void ButtonPress()
    {
        if (pressed == false)
        {
            if (settingMenu == false)
            {
                pressed = true;
                Invoke("PressReset", 1f);
                settingMenu = true;
                GetAnimator().SetBool("SettingsMenu", true);
            }

            else if (settingMenu == true)
            {
                pressed = true;
                Invoke("PressReset", 1f);
                settingMenu = false;
                GetAnimator().SetBool("SettingsMenu", false);
            }
        }
    }

    void PressReset()
    {
        pressed = false;
    }
       

}
