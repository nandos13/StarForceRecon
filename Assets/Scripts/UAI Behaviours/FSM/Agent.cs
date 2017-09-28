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
    public float attackDistance = 20;

    // how much aggro does each squaddie have
    Dictionary<SquadManager.IControllable, float> aggro;

    Health health;

    void Start()
    {
        actions = GetComponents<ActionAI>();
        health = GetComponent<Health>();

        health.OnDamage += Health_OnDamage;
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
        else
            Debug.Log("No action available!");

        // TODO - delete later
        AggroTest();
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

    void AggroTest()
    {
        int index = 0;
        if (Input.GetKeyDown(KeyCode.L))
        {
            print("L was pressed");
            foreach (SquadManager.IControllable s in SquadManager.GetSquadMembers)  // Loop through all squad members
            {

                index++;
            }
        }
    }
}


