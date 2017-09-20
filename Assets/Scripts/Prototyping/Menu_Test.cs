using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Test : MonoBehaviour {

    Animator _animator;
    bool _settings = false;
    bool _exit = false;
    bool pressed = false;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            _animator.SetBool("AnyKey", true);
        }
    }

    public void Settings()
    {
        if (pressed == false)
        {
            if (_settings == false)
            {
                pressed = true;
                Invoke("Reset", 1f);
                _settings = true;
                _animator.SetBool("Settings", true);
            }

            else if (_settings == true)
            {
                pressed = true;
                Invoke("Reset", 1f);
                _settings = false;
                _animator.SetBool("Settings", false);
            }
        }
    }

    public void Exit()
    {
        if (pressed == false)
        {
            if (_exit == false)
            {
                pressed = true;
                Invoke("Reset", 1f);
                _exit = true;
                _animator.SetBool("Exit", true);
            }

            else if (_exit == true)
            {
                pressed = true;
                Invoke("Reset", 1f);
                _exit = false;
                _animator.SetBool("Exit", false);
            }
        }
    }


    void Reset()
    {
        pressed = false;
    }

}
