using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JakePerry
{
    public interface IDamageable
    {
        void ApplyDamage(DamageData data);
    }

    /* TODO: Implement some kind of elemental damage/ resistances system? */
    [System.Serializable]
    public struct DamageData
    {
        private object sender;
        public object Sender { get { return sender; } }

        [SerializeField]
        private float damageValue;
        public float DamageValue { get { return damageValue; } set { damageValue = value; } }

        [SerializeField]
        private DamageLayer.Mask mask;
        /// <summary>A Mask defining which damage layers are affected by this damage.</summary>
        public DamageLayer.Mask DamageMask
        {
            get { return mask; }
            set
            {
                if (value <= 31 && value >= 0)
                    mask = value;
            }
        }
        
        #region Functionality

        public DamageData(object sender, float damageValue, DamageLayer.Mask mask)
        {
            this.sender = sender;
            this.damageValue = damageValue;
            this.mask = mask;
        }

        #endregion
    }
}
