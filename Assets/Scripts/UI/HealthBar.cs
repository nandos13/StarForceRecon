﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StarForceRecon;

public class HealthBar : MonoBehaviour {

    public Health health;
    public Image image;

    public float screenZ;
    public bool inWorld = true;

    Rect initialRect;
    // Use this for initialization
    void Start ()
    {
        if (image && image.type != Image.Type.Filled)
        {
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (health == null)
            return;

        // scale the meter
        float pct = Mathf.Clamp01(health.health / health.maxHealth);
        image.fillAmount = pct * 0.75f;

        if (inWorld)
        {
            transform.position = health.transform.position + Vector3.up * 2;
            transform.forward = Camera.main.transform.forward;

            if (pct <= 0)
                Destroy(gameObject);
        }
        else
        {
            if (health == SquadManager.GetCurrentSquaddie.transform.GetComponent<Health>())
            {
                transform.localScale = Vector3.one * 2.0f;
                transform.SetAsLastSibling();
            }
            else
                transform.localScale = Vector3.one;
        }

    }
}
