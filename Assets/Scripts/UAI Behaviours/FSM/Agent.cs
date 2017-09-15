using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    
    ActionAI[] actions;     

    public ActionAI currentAction;

    public NavMeshAgent nv;

    void Start()
    {
        //// find all enemies on the map, and create an AttackAction for each one
        //Agent[] allAgents = FindObjectsOfType<Agent>();
        //for (int i = 0; i < allAgents.Length; i++)
        //{

        //}





        //// get all the action-derived classes that are siblings of us
        //actions = GetComponents<ActionAI>();
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
}


