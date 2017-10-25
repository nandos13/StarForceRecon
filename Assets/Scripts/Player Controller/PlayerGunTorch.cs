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
            GameController<SFRInputSet> kbm = new KeyboardMouseController<SFRInputSet>(this);
            GameController<SFRInputSet> controller = new DualStickController<SFRInputSet>(this);
        }

        void GameController<SFRInputSet>.ITarget.ReceiveControllerInput(SFRInputSet inputSet, GameController<SFRInputSet>.ControlType controllerType)
        {
            if (flashlight != null && inputSet.GetActionByName("flashlight").WasPressed)
                flashlight.enabled = !flashlight.enabled;
        }
    }
}
