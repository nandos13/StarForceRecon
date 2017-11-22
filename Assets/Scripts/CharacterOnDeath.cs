using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RootMotion.FinalIK;
using StarForceRecon;

public class CharacterOnDeath : MonoBehaviour
{
    [SerializeField, Range(1f, 100f)]
    private float deadBodyDuration = 10.0f;
    Health health;

    private bool IsDead = false;

    void Awake()
    {
        Health h = GetComponentInParent<Health>();
        if (!h) h = GetComponentInChildren<Health>();

        if (h)
            h.OnDeath += OnDeath;

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidbodies)
        {
            if (rb.gameObject == this.gameObject)
                continue;

            rb.isKinematic = true;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            if (col.gameObject == this.gameObject)
                col.enabled = true;
            else
                col.enabled = false;
        }
    }

    private void OnDeath(Health sender, float damageValue)
    {
        IsDead = true;

        ThirdPersonController tpc = GetComponent<ThirdPersonController>();
        tpc.enabled = false;

        AimIK aimIK = GetComponent<AimIK>();
        aimIK.enabled = false;

        Animator anim = GetComponentInChildren<Animator>();
        anim.enabled = false;

        PlayerController pc = GetComponent<PlayerController>();
        pc.enabled = false;

        SquadFollow sf = GetComponent<SquadFollow>();
        sf.enabled = false;

        SquadShooterAI ssAI = GetComponent<SquadShooterAI>();
        ssAI.enabled = false;
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (var col in colliders)
        {
            if (col.gameObject == this.gameObject)
                col.enabled = false;
            else
                col.enabled = true;
        }

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidbodies)
        {
            if (rb.gameObject == this.gameObject)
                rb.isKinematic = true;

            rb.isKinematic = false;
        }

       

        CameraController cameraController = FindObjectOfType<CameraController>();
        cameraController.enabled = false;

        SquadManager.CanSwitchSquadMembers = false;
        //Invoke("GameOver", deadBodyDuration);
    }



    //void GameOver()
    //{
    //    Destroy(this.gameObject);
    //}
}
