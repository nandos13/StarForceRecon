using System.Collections.Generic;
using UnityEngine;

namespace JakePerry
{
    public static class ControllerManager<T, U> where T : GameController<U>, new() 
                                                where U : GameSpecificInputSet, new()
    {
        private static Dictionary<string, T> controllers = new Dictionary<string, T>();

        /// <summary>Gets controller with ID id. Adds target to the controller. If controller does not exist, creates a new one.</summary>
        public static T GetController(string id, GameController<U>.ITarget target)
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

        /// <summary>Does a controller exist with specified id?</summary>
        public static bool HasController(string id)
        {
            return controllers.ContainsKey(id);
        }

        /// <summary>Removes target from controller with ID id</summary>
        public static void RemoveTarget(string id, GameController<U>.ITarget target)
        {
            if (controllers.ContainsKey(id))
                controllers[id].RemoveTarget(target);
        }
    }
}
