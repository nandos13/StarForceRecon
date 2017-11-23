using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueTracker : MonoBehaviour {

    public GameObject _dialogue, _dialogueCanvas;
    public GameObject _skip, _skipCanvas;
    public GameObject skipText;

    bool _active;

    public string _game;

    [System.Serializable]
    public struct ObjectTimeDelay
    {
        [Range(0, 8.0f)]
        public float timeDelay;
        public GameObject obj;
    }

    [SerializeField]
    private List<ObjectTimeDelay> enableObjs = new List<ObjectTimeDelay>();

    // Use this for initialization
    void Awake () {
        StarForceRecon.PlayerController.DisableCursor();
        _active = false;
        TextStart();
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && _active == false)
        {
            TextSkip();
            _active = true;
        }
    }
	
	void TextStart()
    {
        _dialogue.SetActive(true);
        StartCoroutine(DelayedEnable());
    }

    private IEnumerator DelayedEnable()
    {
        foreach (var delayObj in enableObjs)
        {
            yield return new WaitForSeconds(delayObj.timeDelay);
            delayObj.obj.SetActive(true);
        }
        _active = true;
        skipText.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void TextSkip()
    {
        _dialogue.SetActive(false);
        _dialogueCanvas.SetActive(false);
        _skip.SetActive(true);
        _skipCanvas.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void GameStart()
    {
        SceneManager.LoadScene(_game);
    }
}
