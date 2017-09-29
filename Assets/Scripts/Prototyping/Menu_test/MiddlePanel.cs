using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Menu_Test))]
public class MiddlePanel : MonoBehaviour {

    public float _speed;
    private Menu_Test _menuScript;

    bool _middle;
    bool _canvas;

    Animator _animator;
    public GameObject _midPan;

    void Awake()
    {
        Menu_Test _menuScript = gameObject.GetComponent<Menu_Test>();
        _animator = _midPan.GetComponent<Animator>();
        _middle = true;
    }

    [SerializeField]
    public void MiddleActive()
    {
        if (_middle == false)
        {
            _middle = true;
            _animator.SetBool("IsActive", false);
            Invoke("Active", _speed);
        }
        else if(_middle == true)
        {
            _middle = false;
            _animator.SetBool("IsActive", true);
            Invoke("Active", _speed);
        }
    }

    void Active()
    {
        if(_canvas == false)
        {
            _canvas = true;
            _menuScript._menuCanvas.SetActive(false);
        }
        else if(_canvas == true)
        {
            _canvas = false;
            _menuScript._menuCanvas.SetActive(true);
        }
    }
}
