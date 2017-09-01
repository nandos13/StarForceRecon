using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator)), RequireComponent(typeof(AudioPlayer)), RequireComponent(typeof(BoxCollider))]
public class DoorOpenAnimator : MonoBehaviour {

    Animator _animator;
    AudioPlayer _audioPlayer;
    BoxCollider _box;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _audioPlayer = GetComponent<AudioPlayer>();
        _box = GetComponent<BoxCollider>();
    }
    
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            bool isOpen = _animator.GetBool("DoorOpen");
            if (!isOpen)
                _audioPlayer.PlayDelayed("DoorOpen", 0.0f);
            _animator.SetBool("DoorOpen", true);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            // do we have any players left inside?
            bool playersInside = false;
            Collider[] cols = Physics.OverlapBox(_box.bounds.center, _box.size/2, _box.transform.rotation);
            foreach (Collider c in cols)
            {
                if (c.tag == "Player")
                {
                    playersInside = true;
                    break;
                }
            }

            if (playersInside == false)
            {
                _audioPlayer.PlayDelayed("DoorOpen", 0.0f);
                _animator.SetBool("DoorOpen", false);
            }
        }
    }
}
