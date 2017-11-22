using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lvl4light : MonoBehaviour
{
    [System.Serializable]
    public struct ObjectTimeDelay
    {
        [Range(0, 5.0f)]
        public float timeDelay;
        public GameObject obj;
    }

    [SerializeField]
    private List<ObjectTimeDelay> enableObjs = new List<ObjectTimeDelay>();

    private bool triggered = false;

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "Player" && !triggered)
        {
            triggered = true;
            StartCoroutine(DelayedEnable());
        }
    }

    private IEnumerator DelayedEnable()
    {
        foreach (var delayObj in enableObjs)
        {
            yield return new WaitForSeconds(delayObj.timeDelay);
            delayObj.obj.SetActive(true);
        }
    }
}
