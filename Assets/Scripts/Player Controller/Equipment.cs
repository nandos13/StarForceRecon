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

    public IEquipment Primary       { get; set; }
    public IEquipment Secondary     { get; set; }
    public IEquipment Melee         { get; set; }
    public IEquipment Equipment1    { get; set; }
    public IEquipment Equipment2    { get; set; }
    public IEquipment Action1       { get; set; }
    public IEquipment Action2       { get; set; }

    public void Use(Slot slot)
    {
        switch (slot)
        {
            case Slot.Primary:
                Primary.Use(); break;

            case Slot.Secondary:
                Secondary.Use(); break;

            case Slot.Melee:
                Melee.Use(); break;

            case Slot.Equipment1:
                Equipment1.Use(); break;

            case Slot.Equipment2:
                Equipment2.Use(); break;

            case Slot.Action1:
                Action1.Use(); break;

            case Slot.Action2:
                Action2.Use(); break;

            default:
                Debug.LogError("Unexpected Equipment Slot value used as Use parameter", this);
                break;
        }
    }

    /// <summary>Switches primary equipment with secondary equipment.</summary>
    public void PrimarySecondarySwap()
    {
        if (Secondary != null)
        {
            IEquipment temporary = Primary;
            Primary = Secondary;
            Secondary = temporary;
        }
    }
}
