using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(MiddlePanel))]
public class Menu_Test : MonoBehaviour {

    #region Inspector_Information
    Animator _animator;
    public GameObject _camera;
    bool _settings = false;
    bool _exit = false;
    bool _start = false;
    bool pressed = false;
    GameObject _manager;

    [Header("Canvas")]
    public GameObject _anyCanvas;
    public GameObject _menuCanvas;
    public GameObject _settingsCanvas;
    public GameObject _exitCanvas;
    public GameObject _startCanvas;

    [Header("Table")]
    public GameObject _left;
    public GameObject _right;

    public Material _panOn;
    public Material _panOff;

    public Material _light;
    public Material _lightOff;
    public GameObject _table;

    public GameObject sound1;
    public GameObject sound2;

    [Header("Hologram")]
    public GameObject _one;
    public GameObject _two;
    public GameObject _hologram;
    public Animator _holo;
    public float _reset;

    #endregion

    private MiddlePanel _middleScript;

    void Awake()
    {
        _animator = _camera.GetComponent<Animator>();
        _hologram.SetActive(false);
        _settingsCanvas.SetActive(false);
        _startCanvas.SetActive(false);
        _menuCanvas.SetActive(false);
        _exitCanvas.SetActive(false);
        _manager = GameObject.Find("_GAMEMANAGER");
        MiddlePanel _middleScript = _manager.GetComponent<MiddlePanel>();
    }

    //Updating for the hologram
    void Update()
    {
        _hologram.gameObject.transform.Rotate(Vector3.up * -20 * Time.deltaTime);

        if (_hologram.gameObject.transform.forward.z < 0)
        {
            _one.SetActive(false);
            _two.SetActive(true);
        }
        else
        {
            _one.SetActive(true);
            _two.SetActive(false);
        }
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
        _middleScript.MiddleActive();
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
                _left.GetComponent<MeshRenderer>().material = _panOn;
                _settings = true;
                _middleScript.MiddleActive();
                _settingsCanvas.SetActive(true);
            }

            else if (_settings == true)
            {
                pressed = true;
                _animator.SetBool("Settings", false);
                Invoke("Reset", _reset);
                _left.GetComponent<MeshRenderer>().material = _panOff;
                _settings = false;
                _middleScript.MiddleActive();
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
                _middleScript.MiddleActive();
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
                _middleScript.MiddleActive();
            }
        }
    }

    void HoloTest()
    {
        _table.GetComponent<MeshRenderer>().material = _lightOff;
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
                _right.GetComponent<MeshRenderer>().material = _panOn;
                Invoke("Reset", _reset);
                _startCanvas.SetActive(true);
                _menuCanvas.SetActive(false);
            } 
            else if (_start == true)
            {
                pressed = true;
                _animator.SetBool("Start", false);
                Invoke("Reset", _reset);
                _right.GetComponent<MeshRenderer>().material = _panOff;
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
