using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using StarForceRecon;

public class SquadFollow : MonoBehaviour
{
    #region Locator

    /// <summary>Aims to keep squad members spaced out around the player without mobbing behind.</summary>
    public class Placement : MonoBehaviour
    {
        public static Placement instance { get; private set; }

        private Dictionary<Transform, List<SquadFollow>> movers = new Dictionary<Transform, List<SquadFollow>>();

        static Placement()
        {
            GameObject obj = new GameObject("Squad Placement");
            obj.hideFlags = HideFlags.HideAndDontSave;
            instance = obj.AddComponent<Placement>();
        }

        /// <summary>Adds the SquadFollow instance to the list to receive a movement directive.</summary>
        public void RequestDestination(SquadFollow mover, Transform target)
        {
            if (!movers.ContainsKey(target))
                movers[target] = new List<SquadFollow>();

            movers[target].Add(mover);
        }

        public void LateUpdate()
        {
            foreach (var pair in movers)
            {
                int quantity = pair.Value.Count;
                if (quantity <= 1) continue;

                float angleIteration = 360.0f / (float)quantity;
                float currentAngle = 0.0f;
                Vector3 offset = new Vector3(0,0,1);

                for (int i = 0; i < quantity; i++)
                {
                    // Calculate points
                    Vector3 thisOffset = Quaternion.Euler(0, currentAngle, 0) * offset * pair.Value[i].navAgent.stoppingDistance;
                    currentAngle += angleIteration;

                    pair.Value[i].OnReceiveDestination(pair.Key.position + thisOffset);

                    Debug.DrawRay(pair.Key.position + thisOffset, Vector3.up * 10);
                }
            }

            movers.Clear();
        }
    }

    #endregion

    #region Variables

    private NavMeshAgent navAgent = null;
    private ThirdPersonController tpc = null;

    #endregion

    private void Awake()
    {
        navAgent = GetComponentInParent<NavMeshAgent>();
        if (navAgent == null)
            throw new System.Exception(string.Format("{0} script requires a NavMeshAgent component within the parent hierarchy.", this.GetType()));

        navAgent.Warp(transform.position);

        tpc = GetComponentInParent<ThirdPersonController>();
    }

    private void Update()
    {
        SquadManager.IControllable player = SquadManager.GetCurrentSquaddie;
        if (player != null)
            Placement.instance.RequestDestination(this, player.transform);
    }

    public void OnReceiveDestination(Vector3 destination)
    {
        if (Time.frameCount == 0)
            return;
        navAgent.SetDestination(destination);

        if (navAgent.remainingDistance > navAgent.stoppingDistance)
            tpc.Move(navAgent.desiredVelocity, false, false, true);
        else
            tpc.Move(Vector3.zero, false, false);
    }
}
