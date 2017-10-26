using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using StarForceRecon;

public class Agent : MonoBehaviour
{
    ActionAI[] actions;     
    public ActionAI currentAction;
    public NavMeshAgent nv;

    public float enemyLookDistance = 40; // not really needed. just for testing
    public float aggroRange = 20;

    // how much aggro does each squaddie have
    Dictionary<SquadManager.IControllable, float> aggro = new Dictionary<SquadManager.IControllable, float>();

    Health health;

    void Start()
    {
        actions = GetComponents<ActionAI>();
        health = GetComponent<Health>();

        if (health)
            health.OnDamage += Health_OnDamage;         // GETTING ERRORS
    }

    private void Health_OnDamage(Health sender, float damageValue)
    {
        // TODO - need a damage dealer, coming soon
    }

    void Update()
    {
        // find the best action each frame (TODO - not every frame?)
        ActionAI best = GetBestAction();
        // if it’s different from what we were doing, switch the FSM
        if (best != currentAction)
        {
            if (currentAction)
                currentAction.Exit(this);
            currentAction = best;
            if (currentAction)
                currentAction.Enter(this);
        }
        // update the current action
        if (currentAction)
            currentAction.UpdateAction(this);


        // TODO lose aggro over time
        foreach (SquadManager.IControllable s in SquadManager.GetSquadMembers)  // Loop through all squad members
        {
            if (aggro.ContainsKey(s))       // GETTING ERRORS
            {
                aggro[s] -= Time.deltaTime;
                if (aggro[s] < 0)
                    aggro[s] = 0;
            }

        }
    }

    // checks all our available actions and evaluates each one, getting the best
    ActionAI GetBestAction()
    {
        ActionAI action = null;
        float bestValue = 0;
        foreach (ActionAI a in actions)
        {
            float value = a.Evaluate(this);
            if (action == null || value > bestValue)
            {
                action = a;
                bestValue = value;
            }
        }
        return action;
    }

    public float GetAggro(SquadManager.IControllable squaddie)
    {
        if (aggro.ContainsKey(squaddie)) 
            return aggro[squaddie];

        return 0;
    }
}


