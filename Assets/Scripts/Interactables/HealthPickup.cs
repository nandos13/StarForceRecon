using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarForceRecon;
using JakePerry;

public class HealthPickup : MonoBehaviour, Interaction.IInteractable
{
    [SerializeField, Range(1, 5)]
    private int uses = 3;

    [SerializeField]
    private float healingPower = 50;

    private bool valid = true;

    float Interaction.IInteractable.Duration { get { return 0; } }

    Vector3 Interaction.IInteractable.FocusPoint { get { return Vector3.zero; } }

    bool Interaction.IInteractable.ForceCharacterSwap { get { return false; } }

    Interaction.InteractionType Interaction.IInteractable.Type { get { return Interaction.InteractionType.HealthStation; } }

    void Interaction.IInteractable.OnCompleteInteraction()
    {
        if (valid)
        {
            foreach (var member in SquadManager.GetSquadMembers)
            {
                Health h = member.transform.GetComponent<Health>();
                if (h != null)
                {
                    Debug.Log("Should be healing");
                    DamageLayer.Mask mask = new DamageLayer.Mask();
                    mask.SetLayerState(DamageLayer.Utils.NameToLayer("Player"), true);
                    DamageLayer.Modifier modifier = new DamageLayer.Modifier();
                    modifier.SetModifier(DamageLayer.Utils.NameToLayer("Player"), 1.0f);

                    DamageData damage = new DamageData(this, -1.0f * healingPower, mask, modifier);
                    h.ApplyDamage(damage);
                }
            }
        }

        uses--;
        if (uses == 0)
            valid = false;
    }

    void Interaction.IInteractable.OnStartInteraction(Interaction.InteractionInfo info)
    { }
}
