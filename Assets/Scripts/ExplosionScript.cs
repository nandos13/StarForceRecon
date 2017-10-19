using UnityEngine;
using System.Collections;

public class ExplosionScript : Bang {

    public ParticleSystem particles;
    public GameObject blast;
    public AudioClip SoundClip;
    public AudioSource SoundSource;

    [System.Serializable]
    public class BlastSettings
    {
        public float radius = 5.0f;    // radius at which the explosive will effect rigidbodies
        public float power = 10.0f;    // explosive power/force
        public float explosiveLift = 1.0f;     // determine how the explosion reacts. a higher value means rigidbidoes will fly upward
    }

    [System.Serializable]
    public class InputSettings
    {
        public string EXPLOSION_AXIS = "Fire2";
    }

    public BlastSettings explosion = new BlastSettings();
    public InputSettings input = new InputSettings();

    bool explosionInput;
    bool exploded = false;

    void Start()
    {
        explosionInput = false;
      
    }

    void Update()
    {
        
    }

    public override void Execute()
    {
        Detonate();
    }

    void Detonate()
    {
        if (exploded != true)
        {
            if (particles)
            {
                SoundSource.clip = SoundClip;
                SoundSource.Play();
                particles.Play();
                Vector3 explosionPos = blast.transform.position; // position of gameObject
                Collider[] colliders = Physics.OverlapSphere(explosionPos, explosion.radius); // Overlap takes all colliders that are within the Sphere's radius of the gameObject and puts them into an Array[colliders]
                foreach (Collider hit in colliders) // access each collider thats within our Spheres radius
                {
                    Rigidbody rb = hit.GetComponent<Rigidbody>();   // when hit a collider, get the component called rigidbody
                    Ragdoll ragdoll = hit.gameObject.GetComponentInParent<Ragdoll>();
                    if (rb != null)
                    {
                        rb.AddExplosionForce(explosion.power, explosionPos, explosion.radius, explosion.explosiveLift, ForceMode.Impulse);    // if rb has been hit, explosion on force on all rigidbodies
                        if (ragdoll != null)
                            ragdoll.RagdollOn = true;
                    }
                }
                GameObject bombModel;
                bombModel = transform.FindChild("BombSphereModel").gameObject;
                bombModel.SetActive(false);
                exploded = true;
            }
        }
    }
}
