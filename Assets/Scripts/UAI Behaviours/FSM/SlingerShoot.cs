using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingerShoot : MonoBehaviour
{
    public int slingerDmg = 1;
    public float slingRate = 1f;
    public float slingRange = 50f;
    public float slingRangeForce = 100f;
    public Transform slingerRangeEnd;
    private WaitForSeconds slingerDuration = new WaitForSeconds(0.1f);
    private float nextFire;
    private LineRenderer lineRender; 

    void Start()
    {
        lineRender = GetComponent<LineRenderer>();
        
    }

    void Update()
    {
        Throw();
    }

    public void Throw()
    {
        if (Time.time < nextFire)
            return;

        nextFire = Time.time + slingRate;  // set next shot time
        StartCoroutine(slingerEffect());
        RaycastHit hit;
        lineRender.SetPosition(0, slingerRangeEnd.position);
        if (Physics.Raycast(slingerRangeEnd.position, slingerRangeEnd.forward, out hit, slingRange))
        {
            lineRender.SetPosition(1, hit.point);
            HitBox health = hit.collider.GetComponent<HitBox>();

            if (health != null)
            {
                health.Damage(slingerDmg);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * slingRangeForce);
            }
        }
        else
        {
            lineRender.SetPosition(1, slingerRangeEnd.position + (slingerRangeEnd.forward * slingRange));
        }
    }

    public IEnumerator slingerEffect()
    {
        lineRender.enabled = true;
        yield return slingerDuration;
        lineRender.enabled = false;
    }
}
