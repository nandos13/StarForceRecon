using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JakePerry
{
    /* TODO: Implement some kind of elemental damage/ resistances system? */
    [System.Serializable]
    public class DamageData
    {
        #region Variables

        [SerializeField, HideInInspector]
        private float _damageValue;
        [Range(0.0f, 100.0f), SerializeField]
        private float _defaultDamage = 1.0f;

        [SerializeField]
        private DamageLayer.Mask _mask;

        #endregion

        #region Properties
        
        /// <summary>Default damage value, set once at startup. Actual damage dealt will be the value of damageValue.</summary>
        public float defaultDamage { get { return _defaultDamage; } }
        public float damageValue
        {
            get { return _damageValue; }
            set { _damageValue = value; }
        }

        public DamageLayer.Mask damageMask { get { return _mask; } }

        #endregion

        private void OnEnable()
        {
            _damageValue = _defaultDamage;
        }
    }
}
