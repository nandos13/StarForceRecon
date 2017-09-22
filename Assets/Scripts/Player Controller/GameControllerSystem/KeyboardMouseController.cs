﻿using InControl;

namespace JakePerry
{
    public class KeyboardMouseController : GameController
    {
        public KeyboardMouseController(ITarget target) : base(target)
        {
            type = ControlType.KeyboardAndMouse;

            // Set bindings for Movement
            actionSet.Forward.AddDefaultBinding(Key.W);
            actionSet.Back.AddDefaultBinding(Key.S);
            actionSet.Left.AddDefaultBinding(Key.A);
            actionSet.Right.AddDefaultBinding(Key.D);

            // Set bindings for Look
            actionSet.LookForward.AddDefaultBinding(Mouse.PositiveY);
            actionSet.LookBack.AddDefaultBinding(Mouse.NegativeY);
            actionSet.LookLeft.AddDefaultBinding(Mouse.NegativeX);
            actionSet.LookRight.AddDefaultBinding(Mouse.PositiveX);

            // Set bindings for D-Pad
            actionSet.DPadUp.AddDefaultBinding(Key.Key1);
            actionSet.DPadDown.AddDefaultBinding(Key.Key3);
            actionSet.DPadLeft.AddDefaultBinding(Key.Key2);
            actionSet.DPadRight.AddDefaultBinding(Key.Key4);

            // Set bindings for Triggers & Bumpers
            actionSet.Trigger1.AddDefaultBinding(InputControlType.LeftTrigger);
            actionSet.Trigger1.AddDefaultBinding(InputControlType.RightTrigger);
            actionSet.LeftBumper.AddDefaultBinding(InputControlType.LeftBumper);
            actionSet.RightBumper.AddDefaultBinding();

            // Set bindings for Actions
            actionSet.Action1.AddDefaultBinding(Key.Space);
            actionSet.Action2.AddDefaultBinding(Key.F);
            actionSet.Action3.AddDefaultBinding(Key.R);
            actionSet.Action4.AddDefaultBinding(Key.LeftShift);

            // Set bindings for Start & Select
            actionSet.Start.AddDefaultBinding(Key.Escape);
            actionSet.Select.AddDefaultBinding(Key.Tab);
        }
    }
}