using System.Collections.Generic;
using InControl;

namespace JakePerry
{
    public abstract class GameSpecificInputSet : PlayerActionSet
    {
        protected Dictionary<string, PlayerAction> actions =            new Dictionary<string, PlayerAction>();
        protected Dictionary<string, PlayerOneAxisAction> singleAxes =  new Dictionary<string, PlayerOneAxisAction>();
        protected Dictionary<string, PlayerTwoAxisAction> dualAxes =    new Dictionary<string, PlayerTwoAxisAction>();

        /// <summary>Adds new PlayerAction for each string provided in actions.</summary>
        protected void AddInputs(params string[] actions)
        {
            foreach (string name in actions)
            {
                PlayerAction action = CreatePlayerAction(name);

                if (!this.actions.ContainsKey(name))
                    this.actions.Add(name, action);
            }
        }
        
        protected void CreateAxis(string axisName, PlayerAction negativeAction, PlayerAction positiveAction)
        {
            if (negativeAction == null || positiveAction == null) return;

            PlayerOneAxisAction axis = this.CreateOneAxisPlayerAction(negativeAction, positiveAction);
            if (!singleAxes.ContainsKey(axisName))
                singleAxes.Add(axisName, axis);
        }

        protected void CreateAxis(string axisName, PlayerAction negativeXAction, PlayerAction positiveXAction, 
                                                    PlayerAction negativeYAction, PlayerAction positiveYAction)
        {
            if (negativeXAction == null || positiveXAction == null 
                || negativeYAction == null || positiveYAction == null) return;

            PlayerTwoAxisAction axis = this.CreateTwoAxisPlayerAction(negativeXAction, positiveXAction, negativeYAction, positiveYAction);
            if (!dualAxes.ContainsKey(axisName))
                dualAxes.Add(axisName, axis);
        }
        
        /// <returns>PlayerAction with name if it exists, else null.</returns>
        public PlayerAction GetActionByName(string name)
        {
            if (actions.ContainsKey(name))
                return actions[name];
            return null;
        }

        /// <returns>Single-axis with name if it exists, else null.</returns>
        public PlayerOneAxisAction GetSingleAxisByName(string name)
        {
            if (singleAxes.ContainsKey(name))
                return singleAxes[name];
            return null;
        }

        public PlayerTwoAxisAction GetDualAxisByName(string name)
        {
            if (dualAxes.ContainsKey(name))
                return dualAxes[name];
            return null;
        }

        #region AddDefaultBinding overloads

        protected void AddDefaultBinding(string actionName, BindingSource binding)
        {
            PlayerAction action = GetActionByName(actionName);
            if (action != null)
                action.AddDefaultBinding(binding);
        }

        protected void AddDefaultBinding(string actionName, InputControlType control)
        {
            PlayerAction action = GetActionByName(actionName);
            if (action != null)
                action.AddDefaultBinding(control);
        }

        protected void AddDefaultBinding(string actionName, KeyCombo keyCombo)
        {
            PlayerAction action = GetActionByName(actionName);
            if (action != null)
                action.AddDefaultBinding(keyCombo);
        }

        protected void AddDefaultBinding(string actionName, Mouse control)
        {
            PlayerAction action = GetActionByName(actionName);
            if (action != null)
                action.AddDefaultBinding(control);
        }

        protected void AddDefaultBinding(string actionName, params Key[] keys)
        {
            PlayerAction action = GetActionByName(actionName);
            if (action != null)
                action.AddDefaultBinding(keys);
        }

        #endregion
        
        protected virtual void CreateAxes()
        { }

        /// <summary>Sets the default bindings to a dual-stick controller layout.</summary>
        public abstract void CreateDefaultDualStick();

        /// <summary>Sets the default bindings for a keyboard & mouse setup.</summary>
        public abstract void CreateDefaultKeyboardMouse();
    }
}
