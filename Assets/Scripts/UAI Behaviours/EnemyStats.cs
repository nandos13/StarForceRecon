using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JakePerry;

public abstract class EnemyBrain : MonoBehaviour, IDamageable
{
    void IDamageable.ApplyDamage(DamageData data)
    {
        throw new System.NotImplementedException(string.Format("No ApplyDamage function implemented in {0} script", GetType().Name));
    }

    [SerializeField]
    private DamageData damageModule;

    protected abstract void GetNewTarget();


}




/* what does an enemy need to do?
 * be able to pick a target
 * approach the target if too far for attacking
 * actually do the attack (tracking cooldowns, etc).
 * maybe flee
 * 
 */
