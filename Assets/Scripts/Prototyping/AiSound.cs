using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiSound : MonoBehaviour {

    public float minWaitTime;
    public float maxWaitTime;
    public AudioSource flicker_sound;
    public AudioSource dead;

    public void Dead()
    {
        flicker_sound.enabled = false;
        dead.enabled = true;
    }

    void Start()
    {
        flicker_sound = GetComponent<AudioSource>();
        AliveSound();
    }

    void AliveSound()
    {
        if (flicker_sound != null)
            StartCoroutine(Delaying());
    }

    IEnumerator Delaying()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

            if (flicker_sound != null)
                flicker_sound.Play();
        }
    }
}