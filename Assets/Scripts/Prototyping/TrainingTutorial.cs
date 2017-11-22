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
            _isActive = true;
        }
    }

    void Pause()
    {
        Time.timeScale = 0.0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        _canvas.SetActive(true);
    }

    public void GameStart()
    {
        Time.timeScale = 1.0f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(_game);
    }
}
