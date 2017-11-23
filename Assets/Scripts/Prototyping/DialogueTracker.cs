using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTracker : MonoBehaviour {

    public GameObject _dialogue;
    public GameObject _skip;

	// Use this for initialization
	void Awake () {
        _dialogue.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
