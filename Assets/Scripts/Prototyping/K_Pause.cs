﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class K_Pause : MonoBehaviour {

    public GameObject _hudCanvas;
    public GameObject _pauseCanvas;

    public string _menu;

    bool _paused;

    void Start()
    {
        _paused = false;
    }

    void Update()
    {
        if (!_paused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetPauseState(true);
            }
        }

        else if (_paused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetPauseState(false);
            }
        }
    }

    private void SetPauseState(bool state)
    {
        _hudCanvas.SetActive(!state);
        Time.timeScale = state ? 0.0f : 1.0f;
        Cursor.visible = state;
        if (state)
            Cursor.lockState = CursorLockMode.None;
        _pauseCanvas.SetActive(state);
        _paused = state;

        if (state)
            StarForceRecon.PlayerController.DisableCursor();
        else
            StarForceRecon.PlayerController.EnableCursor();
    }

    public void Continue()
    {
        if (_paused)
        {
            SetPauseState(false);
        }
    }

    public void Menu()
    {
        Time.timeScale = 1.0f;
        StarForceRecon.PlayerController.DisableCursor();
        SceneManager.LoadScene(_menu);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
