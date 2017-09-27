using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarForceRecon;

public class EnemyBehaviour : ActionAI
{

    // behavior

    public float targetDistance;
    public float enemyLookDistance; // not really needed. just for testing
    public float attackDistance;
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
        Controlled,
        Idle
    };

    // attack
    public int slingerDmg = 1;
    public float slingRate = 1f;
    public float slingRange = 50f;
    public float slingRangeForce = 100f;
    private float nextFire;
    public Transform slingerRangeEnd;
    public TargetType targetType;
    private WaitForSeconds slingerDuration = new WaitForSeconds(0.1f);
    private LineRenderer lineRender;

    public override float Evaluate(Agent a)
    {
        SquadManager.IControllable target = null;
        switch (targetType)
        {
            case TargetType.Closest: target = closest(); break;
            case TargetType.LowHealth: target = lowestHealth(); break;
            case TargetType.HighHealth: target = highestHealth(); break;
            case TargetType.BiggestThreat: target = greatestThreat(); break;
            case TargetType.Controlled: target = controlled(); break;
            case TargetType.Idle: target = idle(); break;
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
        targetDistance = Vector3.Distance(Target.position, transform.position);
        if (targetDistance < enemyLookDistance)
        {
            lookAtPlayer();
            print("look at player");
        }
        if (targetDistance < attackDistance)
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

        nextFire = Time.time + slingRate;  // set next shot time
        StartCoroutine(slingerEffect());
        RaycastHit hit;
        lineRender.SetPosition(0, slingerRangeEnd.position);
        if (Physics.Raycast(slingerRangeEnd.position, slingerRangeEnd.forward, out hit, slingRange))
        {
            lineRender.SetPosition(1, hit.point);
            Health health = hit.collider.GetComponent<Health>();

            if (health != null)
            {
                //health.ApplyDamage(slingerDmg); needs work
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * slingRangeForce);
            }
        }
        else
        {
            lineRender.SetPosition(1, slingerRangeEnd.position + (slingerRangeEnd.forward * slingRange));
        }
    }

    public IEnumerator slingerEffect()
    {
        lineRender.enabled = true;
        yield return slingerDuration;
        lineRender.enabled = false;
    }
}
