using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionAI : MonoBehaviour
{
    public abstract float Evaluate(Agent a);
    public abstract void UpdateAction(Agent agent);
    public abstract void Enter(Agent agent);
    public abstract void Exit(Agent agent);
    [SerializeField]
    protected float evaluation;

    /// <summary>
    /// Enemy Behaviours
    /// </summary>
    public SquaddieController greatestThreat()  // Brute. approaches greatest threat, does Ground Pound, that does Area Effect Swipe
    {
        SquaddieController best = null;
        return best;
    }

    public SquaddieController highestHealth()   // Approaches Highest health member
    {
        SquaddieController best = null; // Tracker variables to remember which one has the highest health
        float highHealth = 0;
        foreach (SquaddieController s in SquadManager.GetSquadMembers)  // Loop through all squad members
        {
            Health _health = s.GetComponent<Health>();  // Get current squad member's health
            if (_health == null) continue;
            if (_health.healthPercent >= highHealth)    // Check if this squad member has a higher health than any previous members
            {
                highHealth = _health.healthPercent;
                best = s;
                evaluation = highHealth;
            }
        }
        return best;
    }

    public SquaddieController controlled()      // Approuches the players controller member
    {
        SquaddieController best = null;
        return best;
    }     

    public SquaddieController closest()         // Approaches the closest member
    {
        const float closestDist = 3;
        const float farthestDist = 5;

        SquaddieController best = null; // Tracker variables to remember which one is the closest
        float bestDist = 0;
        foreach (SquaddieController s in SquadManager.GetSquadMembers)
        {
            float dist = Vector3.Distance(s.transform.position, transform.position);
            if (best == null || dist < bestDist)    // check if the squad member is the closest
            {
                best = s;
                bestDist = dist;
                evaluation = Mathf.Clamp01(1.0f - (dist-closestDist)/(farthestDist-closestDist));
            }
        }
        return best;
    }

    public SquaddieController lowestHealth()   // slinger
    {
        SquaddieController best = null; // Tracker variables to remember which one has the lowest health
        float lowHealth = float.MaxValue;
        foreach (SquaddieController s in SquadManager.GetSquadMembers)  // Loop through all squad members
        {
            Health _health = s.GetComponent<Health>(); // Get current squad member's health
            if (_health == null) continue;
            if (_health.healthPercent <= lowHealth) // Check if this squad member has a lower health than any previous members
            {
                lowHealth = _health.healthPercent;
                best = s;
                evaluation = 1.0f - lowHealth;
            }
        }
        return best;
    }
}
