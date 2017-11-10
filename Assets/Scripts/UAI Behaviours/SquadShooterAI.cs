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

        [SerializeField, Range(0.0f, 10.0f)]
        private float accurateAngle = 5.0f;

        [SerializeField, Range(0.0f, 1.0f)]
        private float targetEvaluationTime = 0.3f;
        private float timeSinceLastEvaluation = 0.0f;

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
            if (target == null && timeSinceLastEvaluation >= targetEvaluationTime)
                target = GetTarget();
            else
                timeSinceLastEvaluation += Time.deltaTime;

            if (target)
            {
                Vector3 gunForward = gun.GunForward;
                if (gunForward.magnitude > 0)
                {
                    aim.AimAtPoint(target.transform.position);
                    float dot = Vector3.Dot(gunForward, (target.transform.position - gun.transform.position).normalized);
                    if (dot >= Mathf.Cos(accurateAngle * Mathf.Deg2Rad))
                        gun.Fire(false);
                }

                // TODO: NEED A BETTER WAY TO DETERMINE WHEN AGENT IS DEAD??
                if (!target.isActiveAndEnabled)
                    target = null;
            }
        }

        private Agent GetTarget()
        {
            Agent[] agents = FindObjectsOfType<Agent>()
                .Where(a => a.isActiveAndEnabled)
                .Where(a => (Vector3.Distance(a.transform.position, transform.position) <= aggroRange)).ToArray();

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

            timeSinceLastEvaluation = 0.0f;
            return closest;
        }
    }
}
