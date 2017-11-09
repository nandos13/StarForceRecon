using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JakePerry;

namespace StarForceRecon
{
    public class PlayerGunTorch : MonoBehaviour, GameController<SFRInputSet>.ITarget
    {
        [SerializeField]
        private Light flashlight;

        private void Awake()
        {
            ControllerManager<KeyboardMouseController<SFRInputSet>, SFRInputSet>.GetController("PlayerController", this);
            ControllerManager<DualStickController<SFRInputSet>, SFRInputSet>.GetController("PlayerController", this);
        }

        void GameController<SFRInputSet>.ITarget.ReceiveControllerInput(SFRInputSet inputSet, GameController<SFRInputSet>.ControlType controllerType)
        {
            if (flashlight != null && inputSet.GetActionByName("flashlight").WasPressed)
                flashlight.enabled = !flashlight.enabled;
        }
    }
}
