using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyOnDeath : MonoBehaviour
{

    Health health;

    void Awake()
    {
        Health h = GetComponentInParent<Health>();
        if (!h) h = GetComponentInChildren<Health>();

        if (h)
            h.OnDeath += OnDeath;
    }

    private void OnDeath(Health sender, float damageValue)
    {
        Agent a = GetComponent<Agent>();
        Destroy(a);

        EnemyBehaviour eb = GetComponent<EnemyBehaviour>();
        Destroy(eb);

        //Destroy Collider so they dont block player.
        Rigidbody rb = GetComponent<Rigidbody>();
        Destroy(rb);

        Collider c = GetComponent<Collider>();
        Destroy(c);

        NavMeshAgent nv = GetComponent<NavMeshAgent>();
        Destroy(nv);

        Invoke("DeleteEnemy", 10.0f);
    }

    void DeleteEnemy()
    {
        Destroy(this.gameObject);
    }
}
