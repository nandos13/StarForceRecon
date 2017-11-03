﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodOnDeath : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem blood;

    [SerializeField, Range(0.1f, 5f)]
    private float duration = 0.5f;
    

    private static Coroutiner coroutiner;

    private void Awake()
    {
        if (coroutiner == null)
            coroutiner = Coroutiner.Create();
    }

    void Start()
    {
        Health h = GetComponentInParent<Health>();
        if (!h) h = GetComponentInChildren<Health>();

        if (h)
            h.OnDeath += OnDeath;
    }

    private void OnDeath(Health sender, float damageValue)
    {
        if (blood != null)
        {
            ParticleSystem b = Instantiate<ParticleSystem>(blood, transform.position, Quaternion.Euler(-90,0,0));
            coroutiner.StartCoroutine(DestroyParticleAfterSeconds(duration, b));
        }
    }

    private IEnumerator DestroyParticleAfterSeconds(float seconds, ParticleSystem ps)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(ps);
    }

    private class Coroutiner : MonoBehaviour
    {
        public static Coroutiner Create()
        {
            GameObject obj = new GameObject("Coroutiner");
            obj.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(obj);

            return obj.AddComponent<Coroutiner>();
        }
    }
}