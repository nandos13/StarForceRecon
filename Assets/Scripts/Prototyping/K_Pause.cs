using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class K_Pause : MonoBehaviour {

    public GameObject _hudCanvas;
    public GameObject _pauseCanvas;

    bool _paused;

    void Start()
    {
        _paused = false;
    }

    void Update()
    {
        if (_paused == false)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _hudCanvas.SetActive(false);
                Time.timeScale = 0.0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                _pauseCanvas.SetActive(true);
                _paused = true;
            }
        }

        else if (_paused == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _pauseCanvas.SetActive(false);
                Time.timeScale = 1.0f;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                _hudCanvas.SetActive(true);
                _paused = false;
            }
        }
    }

    public void Continue()
    {
        if (_paused == true) {
            _pauseCanvas.SetActive(false);
            Time.timeScale = 1.0f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _hudCanvas.SetActive(true);
            _paused = false;
        }
    }
}
