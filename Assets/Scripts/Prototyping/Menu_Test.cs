using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Test : MonoBehaviour {

    Animator _animator;
    bool _settings = false;
    bool _exit = false;
    bool _start = false;
    bool pressed = false;
    GameObject _anyCanvas, _startCanvas, _menuCanvas;

    public float _reset;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _anyCanvas = GameObject.Find("Anykey");
        _startCanvas = GameObject.Find("StartCanvas");
        _menuCanvas = GameObject.Find("MenuCanvas");
        _menuCanvas.SetActive(false);
        _startCanvas.SetActive(false);
    }

    //There is a button on the "AnyKey Canvas", once pressed this is called.
    public void AnyKey()
    {
        _animator.SetBool("AnyKey", true);
        Destroy(_anyCanvas, 0.6f);
        Invoke("CanvasLoad", 2.3f);
    }

    void CanvasLoad()
    {
        _startCanvas.SetActive(true);
    }

    //When you press setting at the main menu, and then when you press back.
    public void Settings()
    {
        if (pressed == false)
        {
            if (_settings == false)
            {
                pressed = true;
                _animator.SetBool("Settings", true);
                Invoke("Reset", _reset);
                _settings = true;
                _startCanvas.SetActive(false);
                _menuCanvas.SetActive(true);
            }

            else if (_settings == true)
            {
                pressed = true;
                _animator.SetBool("Settings", false);
                Invoke("Reset", _reset);
                _settings = false;
                _startCanvas.SetActive(true);
                _menuCanvas.SetActive(false);
            }
        }
    }

    //when you want to exit the game.
    public void Exit()
    {
        if (pressed == false)
        {
            if (_exit == false)
            {
                pressed = true;
                _animator.SetBool("Exit", true);
                Invoke("Reset", _reset);
                _exit = true;
                _startCanvas.SetActive(false);
            }

            else if (_exit == true)
            {
                pressed = true;
                _animator.SetBool("Exit", false);
                Invoke("Reset", _reset);
                _exit = false;
                _startCanvas.SetActive(true);
            }
        }
    }

    //when you're ready to play/not ready.
    public void MenuStart()
    {
        if (pressed == false)
        {
            if (_start == false)
            {
                pressed = true;
                _animator.SetBool("Start", true);
                Invoke("Reset", _reset);
                _startCanvas.SetActive(false);
            } 
            else if (_start == true)
            {
                pressed = true;
                _animator.SetBool("Start", false);
                Invoke("Reset", _reset);
                _startCanvas.SetActive(true);
            }
        }
    }

    //so that animations dont play right after called.
    void Reset()
    {
        pressed = false;
    }

}
