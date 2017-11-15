using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using StarForceRecon;
using JakePerry;

public class EnemyBehaviour : ActionAI
{

    // behavior
    [Header("Behaviour")]
    [Tooltip("enemies minimum distance to stop movement")]
    public float minDistance;
    [Tooltip("enemies speed of rotation")]
    public float damping;
    public float enemyMoveSpeed;
    public Transform Target;
    Rigidbody _Rigidbody;
    NavMeshAgent agent;

    public enum TargetType
    {
        Closest,
        LowHealth,
        HighHealth,
        BiggestThreat,
        Controlled

    };

    // attack
    [Header("Attacking")]
    [Tooltip("the amount of damage per hit")]
    public int damage = 1;
    [Tooltip("the rate of fire between attacks")]
    public float attackRate = 1f;
    [Tooltip("the distance of range attacks")]
    public float attackRange = 50f;
    [Tooltip("the amount of knock back range force")]
    public float attackRangeForce = 100f;
    private float nextFire;
    [Tooltip("starting location for attacks")]
    public Transform AttackRangeEnd;

    [SerializeField]
    private DamageLayer.Mask damageMask;
    [SerializeField]
    private DamageLayer.Modifier damageModifier;

    [Header("AI Type")]
    public TargetType targetType;
    private WaitForSeconds attackDuration = new WaitForSeconds(0.1f);

    
    public override float Evaluate(Agent a)
    {
        SquadManager.IControllable target = null;
        switch (targetType)
        {
            case TargetType.Closest: target = closest(a); break;
            case TargetType.LowHealth: target = lowestHealth(a); break;
            case TargetType.HighHealth: target = highestHealth(a); break;
            case TargetType.BiggestThreat: target = greatestThreat(a); break;
            case TargetType.Controlled: target = controlled(a); break;
        }

        if (target != null)
            Target = target.transform;
        return evaluation;
    }

    public override void Enter(Agent agent) { }
    public override void Exit(Agent agent) { }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        _Rigidbody = GetComponent<Rigidbody>();
    }
    
    public override void UpdateAction(Agent agent)
    {
        if (Target == null)
        {
            return;
        }
             
        float targetDistance = Vector3.Distance(Target.position, transform.position);
        if (targetDistance < agent.enemyLookDistance)
        {
            lookAtPlayer();
        }
        if (targetDistance < agent.aggroRange)
        {
            moveToPlayer();
            Throw();
        }
    }

    void lookAtPlayer()
    {
        Quaternion rotation = Quaternion.LookRotation(Target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }

    void moveToPlayer()
    {
        agent.SetDestination(Target.position);
    }

    public void Throw()
    {
        if (Time.time < nextFire)
            return;

        nextFire = Time.time + attackRate;  // set next shot time
       
        RaycastHit hit;
        int mask = ~(1 << 9);

        if (Physics.Raycast(AttackRangeEnd.position, AttackRangeEnd.forward, out hit, attackRange, mask))
        {
           
            Health health = hit.collider.GetComponent<Health>();

            if (health != null)
            {
                DamageData damageData = new DamageData(this, damage, damageMask, damageModifier);
                health.ApplyDamage(damageData);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * attackRangeForce);
            }
        }
       
    }
}
