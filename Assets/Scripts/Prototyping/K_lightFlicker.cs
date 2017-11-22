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

        if (flicker_sound != null || flickeringLight != null)
            StartCoroutine(Flickering());
    }

    IEnumerator Flickering()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

            if (flickeringLight != null)
                flickeringLight.enabled = !flickeringLight.enabled;

            if (flicker_sound != null)
                flicker_sound.Play();
        }
    }
}
