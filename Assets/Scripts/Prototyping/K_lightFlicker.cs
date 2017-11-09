using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_lightFlicker : MonoBehaviour {

    Light flickeringLight;
    public float minWaitTime;
    public float maxWaitTime;
    AudioSource flicker_sound;

    void Start()
    {
        flickeringLight = GetComponent<Light>();
        flicker_sound = GetComponent<AudioSource>();
        StartCoroutine(Flickering());
    }

    IEnumerator Flickering()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            flickeringLight.enabled = !flickeringLight.enabled;
            flicker_sound.Play();
        }
    }
}
