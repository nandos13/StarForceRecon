using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueTracker : MonoBehaviour {

    public GameObject _dialogue, _dialogueCanvas;
    public GameObject _skip, _skipCanvas;

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
        TextStart();
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            TextSkip();
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
    }

    void TextSkip()
    {
        _dialogue.SetActive(false);
        _dialogueCanvas.SetActive(false);
        _skip.SetActive(true);
        _skipCanvas.SetActive(true);
    }

    public void GameStart()
    {
        SceneManager.LoadScene(_game);
    }
}
