using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JakePerry
{
    public interface IDamageable
    {
        void ApplyDamage(DamageData data);
    }
}
