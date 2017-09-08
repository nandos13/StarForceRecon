using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JakePerry
{
    public class DamageData : ScriptableObject
    {
        #region Variables

        [Range(0.0f, 100.0f), SerializeField]       private float _damageValue;

        [SerializeField]                            private DamageLayerMask _mask;

        #endregion

        #region Properties
        
        public float damageValue { get { return _damageValue; } }

        public DamageLayerMask damageMask { get { return _mask; } }

        #endregion
    }
}
