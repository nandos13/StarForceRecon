﻿using System.Collections.Generic;
using InControl;
using JakePerry;

namespace StarForceRecon
{
    public class SFRInputSet : GameSpecificInputSet
    {
        public SFRInputSet()
        {
            string[] newInputs = { "moveForward", "moveBack", "moveLeft", "moveRight",
                                    "lookUp", "lookDown", "lookLeft", "lookRight",
                                    "rotateCameraLeft", "rotateCameraRight",
                                    "fire", "reload", "switchForward", "switchBack",
                                    "roll", "melee", "crouch", "switchWeapon",
                                    "equipment1", "equipment2", "action1", "action2", };
            AddInputs(newInputs);

            CreateAxes();
        }

        protected override void CreateAxes()
        {
            CreateAxis("moveVertical", GetActionByName("moveBack"), GetActionByName("moveForward"));
            CreateAxis("moveHorizontal", GetActionByName("moveLeft"), GetActionByName("moveRight"));
            CreateAxis("movement", GetActionByName("moveLeft"), GetActionByName("moveRight"),
                                    GetActionByName("moveBack"), GetActionByName("moveForward"));

            CreateAxis("lookVertical", GetActionByName("lookDown"), GetActionByName("lookUp"));
            CreateAxis("lookHorizontal", GetActionByName("lookLeft"), GetActionByName("lookRight"));
            CreateAxis("looking", GetActionByName("lookLeft"), GetActionByName("lookRight"),
                                    GetActionByName("lookDown"), GetActionByName("lookUp"));
        }

        public override void CreateDefaultDualStick()
        {
            // Set bindings for Movement
            AddDefaultBinding("moveForward", InputControlType.LeftStickUp);
            AddDefaultBinding("moveBack", InputControlType.LeftStickDown);
            AddDefaultBinding("moveLeft", InputControlType.LeftStickLeft);
            AddDefaultBinding("moveRight", InputControlType.LeftStickRight);

            // Set bindings for Look
            AddDefaultBinding("lookUp", InputControlType.RightStickUp);
            AddDefaultBinding("lookDown", InputControlType.RightStickDown);
            AddDefaultBinding("lookLeft", InputControlType.RightStickLeft);
            AddDefaultBinding("lookRight", InputControlType.RightStickRight);

            // Set bindings for D-Pad
            AddDefaultBinding("", InputControlType.DPadUp);
            AddDefaultBinding("", InputControlType.DPadDown);
            AddDefaultBinding("", InputControlType.DPadLeft);
            AddDefaultBinding("", InputControlType.DPadRight);

            // Set bindings for Triggers, Bumpers & Stick buttons
            AddDefaultBinding("fire", InputControlType.RightTrigger);
            AddDefaultBinding("", InputControlType.LeftTrigger);
            AddDefaultBinding("switchBack", InputControlType.LeftBumper);
            AddDefaultBinding("switchForward", InputControlType.RightBumper);
            AddDefaultBinding("", InputControlType.LeftStickButton);
            AddDefaultBinding("crouch", InputControlType.RightStickButton);

            // Set bindings for Actions
            AddDefaultBinding("roll", InputControlType.Action1);
            AddDefaultBinding("melee", InputControlType.Action2);
            AddDefaultBinding("reload", InputControlType.Action3);
            AddDefaultBinding("switchWeapon", InputControlType.Action4);

            // Set bindings for Start & Select
            AddDefaultBinding("", InputControlType.Start);
            AddDefaultBinding("", InputControlType.Select);
        }

        public override void CreateDefaultKeyboardMouse()
        {
            // Set bindings for Movement
            AddDefaultBinding("moveForward", Key.W);
            AddDefaultBinding("moveBack", Key.S);
            AddDefaultBinding("moveLeft", Key.A);
            AddDefaultBinding("moveRight", Key.D);

            // Set bindings for Look
            AddDefaultBinding("lookUp", Mouse.PositiveY);
            AddDefaultBinding("lookDown", Mouse.NegativeY);
            AddDefaultBinding("lookLeft", Mouse.NegativeX);
            AddDefaultBinding("lookRight", Mouse.PositiveX);

            // Set bindings for Number Keys
            AddDefaultBinding("", Key.Key1);
            AddDefaultBinding("", Key.Key3);
            AddDefaultBinding("", Key.Key2);
            AddDefaultBinding("", Key.Key4);
            
            // Set bindings for Actions
            AddDefaultBinding("fire", Mouse.LeftButton);
            AddDefaultBinding("", Mouse.RightButton);
            AddDefaultBinding("rotateCameraLeft", Key.Q);
            AddDefaultBinding("rotateCameraRight", Key.E);
            AddDefaultBinding("", Key.Z);
            AddDefaultBinding("crouch", Key.C);

            AddDefaultBinding("roll", Key.Space);
            AddDefaultBinding("melee", Key.F);
            AddDefaultBinding("reload", Key.R);
            AddDefaultBinding("switchWeapon", Key.Tab);

            AddDefaultBinding("", Key.Escape);
            AddDefaultBinding("", Key.LeftShift);
        }
    }
}