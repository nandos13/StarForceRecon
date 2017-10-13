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
    public static class Locator
    {
        // TODO
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

        tpc = GetComponentInParent<ThirdPersonController>();
    }

    private void Update()
    {
        SquadManager.IControllable player = SquadManager.GetCurrentSquaddie;

        if (player != null)
            navAgent.SetDestination(player.transform.position);

        if (navAgent.remainingDistance > navAgent.stoppingDistance)
            tpc.Move(navAgent.desiredVelocity, false, false);
        else
            tpc.Move(Vector3.zero, false, false);
    }
}
