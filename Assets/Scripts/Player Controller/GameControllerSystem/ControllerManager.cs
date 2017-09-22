using System.Collections.Generic;
using UnityEngine;

namespace JakePerry
{
    public static class ControllerManager<T> where T : GameController, new()
    {
        private static Dictionary<string, T> controllers = new Dictionary<string, T>();

        /// <summary>Gets controller with ID id. Adds target to the controller. If controller does not exist, creates a new one.</summary>
        public static T GetController(string id, GameController.ITarget target)
        {
            if (controllers.ContainsKey(id))
            {
                controllers[id].AddTarget(target);
                return controllers[id];
            }

            T newController = new T();
            newController.AddTarget(target);

            controllers.Add(id, newController);
            return newController;
        }
    }
}
