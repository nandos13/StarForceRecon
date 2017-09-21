using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruteBehavior : ActionAI
{

    public GameObject m_target;
    public float RayCastRange = 10f;


    public override float Evaluate(Agent a)
    {
        // if target is dead, evaluate to zero
        Health hb = m_target.GetComponent<Health>();
        if (hb != null && hb.healthPercent <= 0f)
        {
            return 0.0f;
        }

        //float distance = (m_target.transform.position - a.transform.position).magnitude;
        //if (distance < 8)  // agents range of weapon fire
        //    return 2.0f - hb.healthPercent(); // we prefer attacking enemies closer to death
        else
         return 0.0f;
    }

    public override void UpdateAction(Agent agent)
    {

    }

    public override void Enter(Agent agent)
    {

    }

    public override void Exit(Agent agent)
    {

    }

}
