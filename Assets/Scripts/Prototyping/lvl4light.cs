using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lvl4light : MonoBehaviour {

    public GameObject lights1, lights2, lights3;

    private IEnumerator coroutine;
    bool _active;

    void Start()
    {
        coroutine = WaitAndPrint(1f);
        _active = true;
    }

    void OnTriggerEnter(Collider c) {
        if (c.gameObject.tag == "Player" && _active == true)
        {
            StartCoroutine(coroutine);
            _active = false;
        }
    }
    private IEnumerator WaitAndPrint(float waitTime = 1000f)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            lights1.SetActive(true);
            yield return new WaitForSeconds(waitTime*2);
            lights2.SetActive(true);
            yield return new WaitForSeconds(waitTime*3);
            lights3.SetActive(true);
        }
    }
}
