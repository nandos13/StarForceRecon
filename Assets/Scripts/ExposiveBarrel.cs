
using UnityEngine;
using System.Collections;

public class ExposiveBarrel : MonoBehaviour
{
    public Rigidbody rb;
    public ParticleSystem particles;
    Health health;

    bool exploded = false;

    [System.Serializable]
    public class BlastSettings
    {
        public float radius = 5.0f;    // radius at which the explosive will effect rigidbodies
        public float power = 10.0f;    // explosive power/force
        public float explosiveLift = 1.0f;     // determine how the explosion reacts. a higher value means rigidbidoes will fly upward
    }

    public BlastSettings explosion = new BlastSettings();

    private void Start()
    {
        health = GetComponent<Health>();
        if (health)
            health.OnDeath += OnDeath;
    }

    private void OnDeath(Health sender, float damageValue)
    {
        //damageValue = 100f;
        Detonate();
        Destroy(gameObject);
    }

    void EnableRagdoll()
    {
        rb.isKinematic = false;
        rb.detectCollisions = true;
    }

    void Detonate()
    {
        if (exploded == false)
        {
               
            Vector3 explosionPos = transform.position; // position of gameObject
            Collider[] colliders = Physics.OverlapSphere(explosionPos, explosion.radius); // Overlap takes all colliders that are within the Sphere's radius of the gameObject and puts them into an Array[colliders]
            foreach (Collider hit in colliders) // access each collider thats within our Spheres radius
            {
                rb = hit.GetComponent<Rigidbody>();   // when hit a collider, get the component called rigidbody
                if (rb != null)
                {
                    EnableRagdoll();
                    rb.AddExplosionForce(explosion.power, explosionPos, explosion.radius, explosion.explosiveLift, ForceMode.Impulse);    // if rb has been hit, explosion on force on all rigidbodies
                   // -- needs to do damage on impact. -- //

                }
            }
            exploded = true;
        }
    }
}
