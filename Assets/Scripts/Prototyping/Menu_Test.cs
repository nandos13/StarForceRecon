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
    GameObject _anyCanvas, _menuCanvas, _settingsCanvas, _exitCanvas, _startCanvas;

    GameObject _hologram;

    public GameObject sound1, sound2;

    public Animator _holo;

    public Material _light;
    public Material _lightOff;
    public GameObject _table;

    public float _reset;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _anyCanvas = GameObject.Find("Anykey");
        _menuCanvas = GameObject.Find("MenuCanvas");
        _settingsCanvas = GameObject.Find("SettingsCanvas");
        _exitCanvas = GameObject.Find("ExitCanvas");
        _startCanvas = GameObject.Find("StartCanvas");
        _hologram = GameObject.Find("Hologram");
        _hologram.SetActive(false);
        _settingsCanvas.SetActive(false);
        _startCanvas.SetActive(false);
        _menuCanvas.SetActive(false);
        _exitCanvas.SetActive(false);
    }

    public void Secret()
    {
        sound1.GetComponent<AudioSource>().enabled = false;
        sound2.GetComponent<AudioSource>().enabled = true;
    }

    //There is a button on the "AnyKey Canvas", once pressed this is called.
    public void AnyKey()
    {
        _animator.SetBool("AnyKey", true);
        Destroy(_anyCanvas, 0.6f);
        Invoke("CanvasLoad", 2.8f);
    }

    void CanvasLoad()
    {
        _menuCanvas.SetActive(true);
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
                _menuCanvas.SetActive(false);
                _settingsCanvas.SetActive(true);
            }

            else if (_settings == true)
            {
                pressed = true;
                _animator.SetBool("Settings", false);
                Invoke("Reset", _reset);
                _settings = false;
                _menuCanvas.SetActive(true);
                _settingsCanvas.SetActive(false);
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
                _table.GetComponent<MeshRenderer>().material = _light;
                _exit = true;
                _hologram.SetActive(true);
                _holo.SetBool("Active", true);
                _exitCanvas.SetActive(true);
                _menuCanvas.SetActive(false);
            }

            else if (_exit == true)
            {
                pressed = true;
                _animator.SetBool("Exit", false);
                Invoke("Reset", _reset);
                _holo.SetBool("Active", false);
                Invoke("HoloTest", 0.5f);
                _exit = false;
                _exitCanvas.SetActive(false);
                _menuCanvas.SetActive(true);
            }
        }
    }

    void HoloTest()
    {
        _table.GetComponent<MeshRenderer>().material = _lightOff; _table.GetComponent<MeshRenderer>().material = _lightOff;
        _hologram.SetActive(false);
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
                _start = true;
                Invoke("Reset", _reset);
                _startCanvas.SetActive(true);
                _menuCanvas.SetActive(false);
            } 
            else if (_start == true)
            {
                pressed = true;
                _animator.SetBool("Start", false);
                Invoke("Reset", _reset);
                _start = false;
                _startCanvas.SetActive(false);
                _menuCanvas.SetActive(true);
            }
        }
    }

    //so that animations dont play right after called.
    void Reset()
    {
        pressed = false;
    }

}
