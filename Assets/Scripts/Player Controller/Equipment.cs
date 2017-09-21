using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    [SerializeField]
    private IEquipment primary = null;
    [SerializeField]
    private IEquipment secondary = null;
    [SerializeField]
    private IEquipment melee = null;
    [SerializeField]
    private IEquipment equipment1 = null;
    [SerializeField]
    private IEquipment equipment2 = null;
    [SerializeField]
    private IEquipment action1 = null;
    [SerializeField]
    private IEquipment action2 = null;

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
