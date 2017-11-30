using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundPlay : MonoBehaviour {

    bool _isActive;
    AudioSource sound;

    public float _time;
    public GameObject _text;

    void Start()
    {
            sound = GetComponent<AudioSource>();

        _isActive = false;
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "Player" && _isActive == false)
        {
            sound.Play();
            _text.SetActive(true);
            Destroy(_text, _time);
            _isActive = true;
        }
    }
}
