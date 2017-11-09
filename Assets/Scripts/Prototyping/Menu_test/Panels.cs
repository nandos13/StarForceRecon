using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Menu_Test))]
public class Panels : MonoBehaviour {

    public Menu_Test _menuScript;
    public Animator _table;
    public GameObject _tbObject;

    [Header("IsActive Speeds")]
    public float _speedOn;
    public float _speedOff;

    bool _middle;
    bool _canvasMid;

    bool _left;
    bool _canvasLeft;

    bool _right;
    bool _canvasRight;

    [Header("Panels")]
    public Material _panOn;
    public Material _panOff;
    public GameObject _lTab;
    public GameObject _rTab;

    Animator _animatorMid;
    [Space(10)]
    public GameObject _midPan;

    void Awake()
    {
        _table = _tbObject.GetComponent<Animator>();
        Menu_Test _menuScript = GetComponent<Menu_Test>();
        _animatorMid = _midPan.GetComponent<Animator>();
        _middle = true;
        _left = true;
        _right = true;
    }

    //this works the middle panel, opening lights/canvas load.
    public void MiddleActive()
    {
        if (_middle == true)
        {
            _animatorMid.SetBool("IsActive", true);
            Invoke("MActive", _speedOn);
            _middle = false;
        }
        else if(_middle == false)
        {
            _menuScript._menuCanvas.SetActive(false);
            Invoke("MActive", _speedOff);
            _middle = true;
        }
    }

    void MActive()
    {
        if(_canvasMid == false)
        {
            _menuScript._menuCanvas.SetActive(true);
            _canvasMid = true;
        }
        else if(_canvasMid == true)
        {
            _animatorMid.SetBool("IsActive", false);
            _canvasMid = false;
        }
    }

    //Settings Screen On/off Light Active!
    public void LeftActive()
    {
        if (_left == true)
        {
            _lTab.GetComponent<MeshRenderer>().material = _panOn;
            _table.SetBool("Left", true);
            Invoke("LActive",_speedOn);
            _left = false;
        }
        else if (_left == false)
        {
            _menuScript._settingsCanvas.SetActive(false);
            _table.SetBool("Left", false);
            Invoke("LActive", _speedOff);
            _left = true;
        }
    }

    void LActive()
    {
        if (_canvasLeft == false)
        {
            _menuScript._settingsCanvas.SetActive(true);
            _canvasLeft = true;
        }
        else if (_canvasLeft == true)
        {
            _lTab.GetComponent<MeshRenderer>().material = _panOff;
            _canvasLeft = false;
        }
    }

    //Start Screen On/off Light Active!
    public void RightActive()
    {
        if (_right == true)
        {
            _rTab.GetComponent<MeshRenderer>().material = _panOn;
            _table.SetBool("Right", true);
            Invoke("RActive", _speedOn);
            _right = false;
        }
        else if (_right == false)
        {
            _menuScript._startCanvas.SetActive(false);
            _table.SetBool("Right", false);
            Invoke("RActive", _speedOff);
            _right = true;
        }
    }

    void RActive()
    {
        if (_canvasRight == false)
        {
            _menuScript._startCanvas.SetActive(true);
            _canvasRight = true;
        }
        else if (_canvasRight == true)
        {
            _rTab.GetComponent<MeshRenderer>().material = _panOff;
            _canvasRight = false;
        }
    }

}
