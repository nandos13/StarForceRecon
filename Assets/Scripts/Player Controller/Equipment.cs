using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarForceRecon
{
    public class Equipment : MonoBehaviour
    {
        #region IEquipment interface, Slot enum

        public interface IEquipment
        {
            void Use();
        }

        public enum Slot
        {
            Primary,
            Secondary,
            Melee,
            Equipment1,
            Equipment2,
            Action1,
            Action2
        }

        #endregion

        #region Slots

        private IEquipment primary = null;
        private IEquipment secondary = null;
        private IEquipment melee = null;
        private IEquipment equipment1 = null;
        private IEquipment equipment2 = null;
        private IEquipment action1 = null;
        private IEquipment action2 = null;

        #region MonoBehaviour components for slots
        
        [SerializeField]
        private MonoBehaviour primaryBehaviour = null;
        [SerializeField]
        private MonoBehaviour secondaryBehaviour = null;
        [SerializeField]
        private MonoBehaviour meleeBehaviour = null;
        [SerializeField]
        private MonoBehaviour equipment1Behaviour = null;
        [SerializeField]
        private MonoBehaviour equipment2Behaviour = null;
        [SerializeField]
        private MonoBehaviour action1Behaviour = null;
        [SerializeField]
        private MonoBehaviour action2Behaviour = null;

        #endregion

        #endregion

        private void Awake()
        {
            primary = primaryBehaviour as IEquipment;
            secondary = secondaryBehaviour as IEquipment;
            melee = meleeBehaviour as IEquipment;
            equipment1 = equipment1Behaviour as IEquipment;
            equipment2 = equipment2Behaviour as IEquipment;
            action1 = action1Behaviour as IEquipment;
            action2 = action2Behaviour as IEquipment;
        }

        private void OnValidate()
        {
            if (primaryBehaviour)
            {
                IEquipment temp = primaryBehaviour as IEquipment;
                if (temp != null) primary = temp;
                else primaryBehaviour = null;
            }
            if (secondaryBehaviour)
            {
                IEquipment temp = secondaryBehaviour as IEquipment;
                if (temp != null) secondary = temp;
                else secondaryBehaviour = null;
            }
            if (meleeBehaviour)
            {
                IEquipment temp = meleeBehaviour as IEquipment;
                if (temp != null) melee = temp;
                else meleeBehaviour = null;
            }
            if (equipment1Behaviour)
            {
                IEquipment temp = equipment1Behaviour as IEquipment;
                if (temp != null) equipment1 = temp;
                else equipment1Behaviour = null;
            }
            if (equipment2Behaviour)
            {
                IEquipment temp = equipment2Behaviour as IEquipment;
                if (temp != null) equipment2 = temp;
                else equipment2Behaviour = null;
            }
            if (action1Behaviour)
            {
                IEquipment temp = action1Behaviour as IEquipment;
                if (temp != null) action1 = temp;
                else action1Behaviour = null;
            }
            if (action2Behaviour)
            {
                IEquipment temp = action2Behaviour as IEquipment;
                if (temp != null) action2 = temp;
                else action2Behaviour = null;
            }
        }

        public void Use(Slot slot)
        {
            switch (slot)
            {
                case Slot.Primary:
                    if (primary != null)
                        primary.Use(); break;

                case Slot.Secondary:
                    if (secondary != null)
                        secondary.Use(); break;

                case Slot.Melee:
                    if (melee != null)
                        melee.Use(); break;

                case Slot.Equipment1:
                    if (equipment1 != null)
                        equipment1.Use(); break;

                case Slot.Equipment2:
                    if (equipment2 != null)
                        equipment2.Use(); break;

                case Slot.Action1:
                    if (action1 != null)
                        action1.Use(); break;

                case Slot.Action2:
                    if (action2 != null)
                        action2.Use(); break;

                default:
                    Debug.LogError("Unexpected Equipment Slot value used as Use parameter", this);
                    break;
            }
        }

        /// <summary>Switches primary equipment with secondary equipment.</summary>
        public void PrimarySecondarySwap()
        {
            if (secondary != null)
            {
                IEquipment temporary = primary;
                primary = secondary;
                secondary = temporary;
            }
        }
    }
}
