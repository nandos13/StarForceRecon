using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using StarForceRecon;

public class SquadFollow : MonoBehaviour
{
    #region Variables

    private NavMeshAgent navAgent = null;
    private ThirdPersonController tpc = null;
    private SquadManager.IControllable currentPlayer = null;

    #endregion

    private void Awake()
    {
        navAgent = GetComponentInParent<NavMeshAgent>();
        if (navAgent == null)
            throw new System.Exception(string.Format("{0} script requires a NavMeshAgent component within the parent hierarchy.", this.GetType()));

        navAgent.Warp(transform.position);

        tpc = GetComponentInParent<ThirdPersonController>();
        currentPlayer = SquadManager.GetCurrentSquaddie;
        SquadManager.OnSwitchSquaddie += SquadManager_OnSwitchSquaddie;
    }

    private void SquadManager_OnSwitchSquaddie()
    {
        currentPlayer = SquadManager.GetCurrentSquaddie;
    }

    private void Update()
    {
        if (currentPlayer != null)
            navAgent.SetDestination(currentPlayer.transform.position);
    }
}
