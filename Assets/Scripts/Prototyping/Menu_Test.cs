using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Test : MonoBehaviour {

    Animator _animator;
    bool _settings = false;
    bool _exit = false;
    bool pressed = false;
    GameObject _anyCanvas, _startCanvas, _menuCanvas;

    public GameObject _door;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _anyCanvas = GameObject.Find("Anykey");
        _startCanvas = GameObject.Find("StartCanvas");
        _menuCanvas = GameObject.Find("MenuCanvas");
        _menuCanvas.SetActive(false);
    }

    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            _animator.SetBool("AnyKey", true);
            Destroy(_anyCanvas, 0.5f);
        }
    }

    public void Settings()
    {
        if (pressed == false)
        {
            if (_settings == false)
            {
                pressed = true;
                _animator.SetBool("Settings", true);
                Invoke("Reset", 1.0f);
                _settings = true;
                _startCanvas.SetActive(false);
                _menuCanvas.SetActive(true);
            }

            else if (_settings == true)
            {
                pressed = true;
                _animator.SetBool("Settings", false);
                Invoke("Reset", 1.0f);
                _settings = false;
                _startCanvas.SetActive(true);
                _menuCanvas.SetActive(false);
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
                _animator.SetBool("Exit", true);
                Invoke("Reset", 1.0f);
                _exit = true;
                _startCanvas.SetActive(false);
            }

            else if (_exit == true)
            {
                pressed = true;
                _animator.SetBool("Exit", false);
                Invoke("Reset", 1.0f);
                _exit = false;
                _startCanvas.SetActive(true);
            }
        }
    }

    public void GameStart()
    {
        if (pressed == false)
        {
            pressed = true;
            _animator.SetBool("Start", true);
            _startCanvas.SetActive(false);
        }
    }


    void DoorActivate()
    {
        _door.GetComponent<BoxCollider>().enabled = true;
    }

    void Reset()
    {
        pressed = false;
    }

}
