using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is the action for attacking a particular enemy unit specified by the m_target member
public class AttackAction : ActionAI
{
    public GameObject m_target;
    public float RayCastRange = 12.0f;
    public override float Evaluate(Agent a)
    {
        // if target is dead, evaluate to zero
        HitBox hb = m_target.GetComponent<HitBox>();
        if (hb != null && hb.currentHealth <= 35f)
        {
            return 0.0f;
        }

        float distance = (m_target.transform.position - a.transform.position).magnitude;
        if (distance < 8)  // agents range of weapon fire
            return 2.0f - hb.GetPercentHealth(); // we prefer attacking enemies closer to death
        else
            return 0.0f;
    }
   
    public override void UpdateAction(Agent agent)
    {
        RaycastHit _hitInfo;
        Vector3 direction = m_target.transform.position - agent.transform.position;
        direction.y = 0;
        direction.Normalize();
        if (Physics.Raycast(agent.transform.position, direction, out _hitInfo, RayCastRange))
        {
            if (_hitInfo.transform.gameObject != m_target)
            {
                //if (agent.nv.currentPath.Count == 0)
                    agent.nv.SetDestination(m_target.transform.position);
            }
            else
            {

                SlingerShoot shoot = agent.GetComponent<SlingerShoot>();
                if (shoot)// fire weapons
                {
                    agent.transform.forward = m_target.transform.position - agent.transform.position;
                    shoot.Throw();
                }
            }
        }
    }

    public override void Enter(Agent agent)
    {
        // stop and start firing
        agent.nv.Stop();
    }

    public override void Exit(Agent agent) { }
}
