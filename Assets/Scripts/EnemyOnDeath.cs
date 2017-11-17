using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyOnDeath : MonoBehaviour
{
    [SerializeField,Range(1f, 100f)]
    private float deadBodyDuration = 10.0f;
    Health health;

    private bool IsDead = false;
    private float Delay = 0.3f;

    void Awake()
    {
        Health h = GetComponentInParent<Health>();
        if (!h) h = GetComponentInChildren<Health>();

        if (h)
            h.OnDeath += OnDeath;
    }

    private void OnDestroy()
    {
        Collider c = GetComponent<Collider>();
        if (c) Destroy(c);

        Rigidbody rb = GetComponent<Rigidbody>();
        Destroy(rb);
    }

    private void OnDeath(Health sender, float damageValue)
    {
        IsDead = true;

        Agent a = GetComponent<Agent>();
        Destroy(a);

        EnemyBehaviour eb = GetComponent<EnemyBehaviour>();
        Destroy(eb);

        Animator anim = GetComponent<Animator>();
        Destroy(anim);
        
        NavMeshAgent nv = GetComponent<NavMeshAgent>();
        Destroy(nv);

        //Collider c = GetComponent<Collider>();
        //Destroy(c);

        //Rigidbody rb = GetComponent<Rigidbody>();
        //Destroy(rb);

        Invoke("DeleteEnemy", deadBodyDuration);
    }

    private void Update()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (IsDead && rb.velocity.magnitude < 0.0001f)
        {
            if (Delay > 0)
                Delay -= Time.deltaTime;
            else
            {
                rb.isKinematic = true;
                rb.detectCollisions = false;
            } 
        }
    }

    void DeleteEnemy()
    {
        Destroy(this.gameObject);
    }
}
