using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarForceRecon;

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
    [Header("AI Type")]
    public TargetType targetType;
    private WaitForSeconds attackDuration = new WaitForSeconds(0.1f);
    private LineRenderer lineRender;
   
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

    void Start()
    {
        lineRender = GetComponent<LineRenderer>();
        _Rigidbody = GetComponent<Rigidbody>();
    }
    
    public override void UpdateAction(Agent agent)
    {
        if (Target == null)
        {
            Debug.Log("no Target!");
            return;
        }
             
        float targetDistance = Vector3.Distance(Target.position, transform.position);
        if (targetDistance < agent.enemyLookDistance)
        {
            lookAtPlayer();
            print("look at player");
        }
        if (targetDistance < agent.aggroRange)
        {
            attackAtPlayer();
            Throw();
            print("attack!");
        }
    }

    void lookAtPlayer()
    {
        Quaternion rotation = Quaternion.LookRotation(Target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }

    void attackAtPlayer()
    {
        _Rigidbody.velocity = (transform.forward * enemyMoveSpeed);
        if (Vector3.Distance(transform.position, Target.position) < minDistance)
        {
            _Rigidbody.velocity = Vector3.zero;
        }
    }

    public void Throw()
    {
        if (Time.time < nextFire)
            return;

        nextFire = Time.time + attackRate;  // set next shot time
        StartCoroutine(slingerEffect());
        RaycastHit hit;
        lineRender.SetPosition(0, AttackRangeEnd.position);
        if (Physics.Raycast(AttackRangeEnd.position, AttackRangeEnd.forward, out hit, attackRange))
        {
            lineRender.SetPosition(1, hit.point);
            Health health = hit.collider.GetComponent<Health>();

            if (health != null)
            {
                //health.ApplyDamage(damage); needs work
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * attackRangeForce);
            }
        }
        else
        {
            lineRender.SetPosition(1, AttackRangeEnd.position + (AttackRangeEnd.forward * attackRange));
        }
    }

    public IEnumerator slingerEffect()
    {
        lineRender.enabled = true;
        yield return attackDuration;
        lineRender.enabled = false;
    }
}
