using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    // TODO: Change to DamageData rather than float parameter
    void ApplyDamage(float damage);
}
