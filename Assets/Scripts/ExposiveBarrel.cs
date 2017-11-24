using JakePerry;
using UnityEngine;
using System.Collections;

public class ExposiveBarrel : MonoBehaviour
{
    [SerializeField]
    private float damage = 50;

    [SerializeField]
    private GameObject standard;
    [SerializeField]
    private GameObject destroyed;
   
    bool exploded = false;

    [System.Serializable]
    public class BlastSettings
    {
        public float radius = 5.0f;    // radius at which the explosive will effect rigidbodies
        public float power = 10.0f;    // explosive power/force
        public float explosiveLift = 10.0f;     // determine how the explosion reacts. a higher value means rigidbidoes will fly upward
    }

    public BlastSettings explosion = new BlastSettings();

    private void Awake()
    {
        if (!standard || !destroyed)
            throw new System.NullReferenceException();

        standard.SetActive(true);
        destroyed.SetActive(false);
    }

    private void Start()
    {
        Health health = GetComponent<Health>();
        if (health)
            health.OnDeath += OnDeath;
    }

    private void OnDeath(Health sender, float damageValue)
    {
        Detonate();
    }

    void Detonate()
    {
        if (exploded == false)
        {
            standard.SetActive(false);
            destroyed.SetActive(true);

            Collider[] colliders = Physics.OverlapSphere(transform.position, explosion.radius);
            foreach (var col in colliders)
            {
                // Apply knockback
                Rigidbody rb = col.GetComponent<Rigidbody>();
                if (rb)
                    rb.AddExplosionForce(explosion.power, transform.position, explosion.radius);

                // Apply damage
                Health health = col.GetComponent<Health>();
                if (health)
                {
                    DamageLayer.Mask mask = new DamageLayer.Mask();
                    mask.SetLayerState(DamageLayer.Utils.NameToLayer("Enemy"), true);
                    mask.SetLayerState(DamageLayer.Utils.NameToLayer("Player"), true);

                    DamageLayer.Modifier modifier = new DamageLayer.Modifier();
                    modifier.SetModifier(DamageLayer.Utils.NameToLayer("Enemy"), 1.0f);
                    modifier.SetModifier(DamageLayer.Utils.NameToLayer("Player"), 0.5f);

                    DamageData damageData = new DamageData(this, damage, mask, modifier);
                    health.ApplyDamage(damageData);
                }
            }

            exploded = true;
        }
    }
}
