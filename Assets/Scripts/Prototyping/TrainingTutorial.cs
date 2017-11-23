using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainingTutorial : MonoBehaviour {

    public GameObject beforeSkip;
    Animator _aSkip;
    AudioSource _sSkip;

    AudioSource _final;

    bool _isActive;

    public GameObject real, fake;

    public GameObject _canvas;

    public string _game;

    void Awake()
    {
        _aSkip = beforeSkip.GetComponent<Animator>();
        _sSkip = beforeSkip.GetComponent<AudioSource>();
        _final = GetComponent<AudioSource>();
        _isActive = false;

        StarForceRecon.PlayerController.DisableCursor();

        Invoke("Game", 0.3f);
    }

    void Game()
    {
        _aSkip.SetBool("Active", true);
        _sSkip.Play();
        real.SetActive(false);
        fake.SetActive(true);
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "Player" && _isActive == false)
        {
            _final.Play();
            Invoke("Pause", 2f);
            Invoke("GameStart", 4f);
            _isActive = true;
        }
    }

    void Pause()
    {
        _canvas.SetActive(true);
    }

    void GameStart()
    {
        SceneManager.LoadScene(_game);
    }
}
