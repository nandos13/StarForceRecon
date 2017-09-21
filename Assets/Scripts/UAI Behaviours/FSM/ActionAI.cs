using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionAI : MonoBehaviour
{
    public abstract float Evaluate(Agent a);
    public abstract void UpdateAction(Agent agent);
    public abstract void Enter(Agent agent);
    public abstract void Exit(Agent agent);

    //public SquaddieController greatestThreat() {}  // Brute. approaches greatest threat, does Ground Pound, that does Area Effect Swipe
  

    public SquaddieController highestHealth()  // Approaches Highest health member
    {
        // Tracker variables to remember which one has the highest health
        SquaddieController best = null;
        float highHealth = 0;

        // Loop through all squad members
        foreach (SquaddieController s in SquadManager.GetSquadMembers)
        {
            // Get current squad member's health
            Health _health = s.GetComponent<Health>();
            if (_health == null) continue;

            // Check if this squad member has a higher health than any previous members
            if (_health.healthPercent >= highHealth)
            {
                highHealth = _health.healthPercent;
                best = s;
            }
        }

        // Return the lowest health member that we found
        return best;
    }

    //public SquaddieController controlled(){}     // Approuches the players controller member


    public SquaddieController closest()         // Approaches the closest member
    {
        // Tracker variables to remember which one is the closest
        SquaddieController best = null;
        float bestDist = 0;

        foreach (SquaddieController s in SquadManager.GetSquadMembers)
        {
            float dist = Vector3.Distance(s.transform.position, transform.position);
            // check if the squad member is the closest
            if (best == null || dist < bestDist)
            {
                best = s;
                bestDist = dist;
            }
        }

        // returns the closest member that we found
        return best;
    }

    public SquaddieController lowestHealth()   // slinger
    {
        // Tracker variables to remember which one has the lowest health
        SquaddieController best = null;
        float lowHealth = float.MaxValue;

        // Loop through all squad members
        foreach (SquaddieController s in SquadManager.GetSquadMembers)
        {
            // Get current squad member's health
            Health _health = s.GetComponent<Health>();
            if (_health == null) continue;
            
            // Check if this squad member has a lower health than any previous members
            if (_health.healthPercent <= lowHealth)
            {
                lowHealth = _health.healthPercent;
                best = s;
            }
        }

        // Return the lowest health member that we found
        return best;
    }

}
