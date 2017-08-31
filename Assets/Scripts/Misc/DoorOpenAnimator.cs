using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenAnimator : MonoBehaviour {

     
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            gameObject.GetComponent<Animator>().SetBool("DoorOpen", true);
          
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
                gameObject.GetComponent<Animator>().SetBool("DoorOpen", false);
          
        }
    }
}
