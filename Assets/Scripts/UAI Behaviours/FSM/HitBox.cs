using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{

    public int currentHealth = 100;
    private int maxHealth;

    void Start()
    {
        maxHealth = currentHealth;
    }

    public float GetPercentHealth()
    {
        return ((float)currentHealth) / ((float)maxHealth);
    }

    public void Damage(int damageAmount)
    {
        // substract damage amount when damage function is called
        currentHealth -= damageAmount;
    }
}
