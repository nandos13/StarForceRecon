using InControl;

namespace JakePerry
{
    public class DualStickController : GameController
    {
        public DualStickController(ITarget target) : base(target)
        {
            // Set bindings for Movement
            actionSet.Forward.AddDefaultBinding(InputControlType.LeftStickUp);
            actionSet.Back.AddDefaultBinding(InputControlType.LeftStickDown);
            actionSet.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
            actionSet.Right.AddDefaultBinding(InputControlType.LeftStickRight);

            // Set bindings for Look
            actionSet.LookForwardAnalog.AddDefaultBinding(InputControlType.RightStickUp);
            actionSet.LookBackAnalog.AddDefaultBinding(InputControlType.RightStickDown);
            actionSet.LookLeftAnalog.AddDefaultBinding(InputControlType.RightStickLeft);
            actionSet.LookRightAnalog.AddDefaultBinding(InputControlType.RightStickRight);

            // Set bindings for D-Pad
            actionSet.DPadUp.AddDefaultBinding(InputControlType.DPadUp);
            actionSet.DPadDown.AddDefaultBinding(InputControlType.DPadDown);
            actionSet.DPadLeft.AddDefaultBinding(InputControlType.DPadLeft);
            actionSet.DPadRight.AddDefaultBinding(InputControlType.DPadRight);

            // Set bindings for Triggers & Bumpers
            actionSet.Trigger1.AddDefaultBinding(InputControlType.LeftTrigger);
            actionSet.Trigger1.AddDefaultBinding(InputControlType.RightTrigger);
            actionSet.LeftBumper.AddDefaultBinding(InputControlType.LeftBumper);
            actionSet.RightBumper.AddDefaultBinding(InputControlType.RightBumper);

            // Set bindings for Actions
            actionSet.Action1.AddDefaultBinding(InputControlType.Action1);
            actionSet.Action2.AddDefaultBinding(InputControlType.Action2);
            actionSet.Action3.AddDefaultBinding(InputControlType.Action3);
            actionSet.Action4.AddDefaultBinding(InputControlType.Action4);

            // Set bindings for Start & Select
            actionSet.Start.AddDefaultBinding(InputControlType.Start);
            actionSet.Select.AddDefaultBinding(InputControlType.Select);
        }
    }
}
