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
        public float RawDamageValue
        {
            get { return damageValue; }
            set { damageValue = value; }
        }

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

        public DamageLayer.Modifier modifier;
        /// <summary>Damage modifiers for layers.</summary>
        public DamageLayer.Modifier DamageModifier
        {
            get { return modifier; }
            set { modifier = value; }
        }
        
        #region Functionality

        public DamageData(object sender, float damageValue, DamageLayer.Mask mask, DamageLayer.Modifier modifier)
        {
            this.sender = sender;
            this.damageValue = damageValue;
            this.mask = mask;
            this.modifier = modifier;
        }

        public float DamageValue(DamageLayer layer)
        {
            return damageValue * modifier.GetModifier(layer);
        }

        #endregion
    }
}
