using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StarForceRecon
{
    [RequireComponent(typeof(AimHandler))]
    public class SquadShooterAI : MonoBehaviour
    {
        [SerializeField, Range(2.0f, 15.0f)]
        private float aggroRange = 10.0f;

        private AimHandler aim = null;
        private Gun gun = null;

        private Agent target = null;

        private void Awake()
        {
            aim = GetComponent<AimHandler>();
            gun = GetComponentInChildren<Gun>();
            if (gun == null)
                throw new System.Exception("SquadShooterAI script requires a Gun script");
        }

        private void Update()
        {
            if (target == null)
                target = GetTarget();

            if (target)
            {
                aim.AimAtPoint(target.transform.position);
                gun.Fire(false);
            }
        }

        private Agent GetTarget()
        {
            Agent[] agents = FindObjectsOfType<Agent>().Where(a => (Vector3.Distance(a.transform.position, transform.position) <= aggroRange)).ToArray();

            // Get closest
            Agent closest = null;
            float dist = float.MaxValue;
            foreach (Agent a in agents)
            {
                float thisDist = Vector3.Distance(a.transform.position, transform.position);
                if (thisDist < dist)
                {
                    dist = thisDist;
                    closest = a;
                }
            }

            return closest;
        }
    }
}
