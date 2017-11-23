using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Panels))]
public class Menu_Test : MonoBehaviour {

    #region Inspector_Information
    Animator _animator;
    public GameObject _camera;
    bool _settings = false;
    bool _exit = false;
    bool _start = false;
    bool pressed = false;
    public Panels _middleScript;
    public string _levelName;
    public string _training;

    bool _anyKey;

    [Header("Canvas")]
    public GameObject _anyCanvas;
    public GameObject _menuCanvas;
    public GameObject _settingsCanvas;
    public GameObject _exitCanvas;
    public GameObject _startCanvas;

    [Header("Table")]
    public Material _light;
    public Material _lightOff;
    public GameObject _table;

    [Header("Hologram")]
    public GameObject _one;
    public GameObject _two;
    public GameObject _hologram;
    public Animator _holo;
    public float _reset;

    #endregion

    void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _animator = _camera.GetComponent<Animator>();
        _hologram.SetActive(false);
        _settingsCanvas.SetActive(false);
        _startCanvas.SetActive(false);
        _menuCanvas.SetActive(false);
        _exitCanvas.SetActive(false);
        Panels _middleScript = GetComponent<Panels>();
        _anyKey = false;
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

        if (Input.anyKeyDown && _anyKey == false)
        {
            AnyKey();
            _anyKey = true;
        }
    }

    //There is a button on the "AnyKey Canvas", once pressed this is called.
    public void AnyKey()
    {
        _animator.SetBool("AnyKey", true);
        Destroy(_anyCanvas);
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
                _middleScript.MiddleActive();
                _middleScript.LeftActive();
                _settings = true;
            }

            else if (_settings == true)
            {
                pressed = true;
                _animator.SetBool("Settings", false);
                Invoke("Reset", _reset);
                _middleScript.MiddleActive();
                _middleScript.LeftActive();
                _settings = false;
            }
        }
    }

    //when you want to exit the game "animation".
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
                _hologram.SetActive(true);
                _holo.SetBool("Active", true);
                _exitCanvas.SetActive(true);
                _middleScript.MiddleActive();
                _exit = true;
            }

            else if (_exit == true)
            {
                pressed = true;
                _animator.SetBool("Exit", false);
                Invoke("Reset", _reset);
                _holo.SetBool("Active", false);
                Invoke("HoloTest", 0.5f);
                _exitCanvas.SetActive(false);
                _middleScript.MiddleActive();
                _exit = false;
            }
        }
    }

    public void Quit()
    {
        Application.Quit();
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
                Invoke("Reset", _reset);
                _middleScript.RightActive();
                _middleScript.MiddleActive();
                _start = true;
            } 
            else if (_start == true)
            {
                pressed = true;
                _animator.SetBool("Start", false);
                Invoke("Reset", _reset);
                _middleScript.RightActive();
                _middleScript.MiddleActive();
                _start = false;
            }
        }
    }

    public void GameStart()
    {
        SceneManager.LoadScene(_levelName);
    }

    public void TrainingStart()
    {
        SceneManager.LoadScene(_training);
    }

    //so that animations dont play right after called.
    void Reset()
    {
        pressed = false;
    }

}
