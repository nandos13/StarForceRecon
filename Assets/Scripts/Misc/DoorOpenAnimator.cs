using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenAnimator : MonoBehaviour {

    Animator animator;

    Animator GetAnimator()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        return animator;
    }


    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            bool isOpen = GetAnimator().GetBool("DoorOpen");
            if (!isOpen)
                gameObject.GetComponent<AudioSource>().Play();
            GetAnimator().SetBool("DoorOpen", true);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            // do we have any players left inside?
            bool playersInside = false;
            BoxCollider box = GetComponent<BoxCollider>();
            Collider[] cols = Physics.OverlapBox(box.bounds.center, box.size/2, box.transform.rotation);
            foreach (Collider c in cols)
                if (c.tag == "Player")
                    playersInside = true;

            if (playersInside == false)
            {
                gameObject.GetComponent<AudioSource>().Play();
                GetAnimator().SetBool("DoorOpen", false);
            }
        }
    }
}
