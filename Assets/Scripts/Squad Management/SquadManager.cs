using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StarForceRecon
{
    public static class SquadManager
    {
        #region Switch event, IControllable interface

        public delegate void ControllableDestroy(IControllable sender);

        public interface IControllable
        {
            void OnSwitchTo();
            void OnSwitchAway();

            /// <summary>Is the character currently selectable?</summary>
            bool Controllable { get; }

            UnityEngine.Transform transform { get; }
            
            /// <summary>Should be raised when the object is destroyed.</summary>
            event ControllableDestroy OnControlTargetDestroyed;
        }

        public delegate void SquadMemberSwitchEvent();
        public static event SquadMemberSwitchEvent OnSwitchSquaddie;  // This event is called every time the selected squad member is changed

        #endregion

        #region Squad Selection

        private static bool _canSwitchSquadMembers = true;
        public static bool CanSwitchSquadMembers
        {
            get { return _canSwitchSquadMembers; }
            set { _canSwitchSquadMembers = value; }
        }

        private static List<IControllable> _squadMembers = new List<IControllable>();
        private static IControllable selected = null;

        public static List<IControllable> GetSquadMembers { get { return _squadMembers; } }

        public static IControllable GetCurrentSquaddie { get { return selected; } }

        #endregion

        private static void RaiseSwitchEvent()
        {
            if (OnSwitchSquaddie != null)    // Check there are registered listeners before firing event 
                OnSwitchSquaddie();
        }

        /// <summary>Adds the specified member to the squad.</summary>
        public static void AddSquadMember(IControllable member)
        {
            if (member != null)
            {
                int currentMembers = _squadMembers.Count;
                _squadMembers.Add(member);
                member.OnControlTargetDestroyed += TargetDestroyed;

                // If no characters are selected
                if (selected == null)
                {
                    selected = member;

                    member.OnSwitchTo();
                    RaiseSwitchEvent();
                }
                else
                    member.OnSwitchAway();
            }
        }

        private static void TargetDestroyed(IControllable sender)
        {
            _squadMembers.Remove(sender);
            if (selected == sender)
                Switch();
        }

        private static void DoSwitch(IControllable to)
        {
            selected.OnSwitchAway();
            selected = to;
            selected.OnSwitchTo();
            RaiseSwitchEvent();
        }
        
        /// <summary>Switches to the next available character. Will iterate backwards if reverse is true.</summary>
        public static void Switch(bool reverse = false)
        {
            if (!_canSwitchSquadMembers) return;
            if (_squadMembers.Count == 0)
            {
                selected = null;
                return;
            }

            // Get a list of the currently selectable members
            List<IControllable> selectable = _squadMembers.Where(character => character.Controllable).ToList();
            if (selectable.Count == 0) return;
            
            if (selectable.Count == 1 && selected != selectable[0])
            {
                selected.OnSwitchAway();
                selected = selectable[0];
                selected.OnSwitchTo();
            }
            
            IControllable next = null;
            int nextIndex;

            if (selectable.Contains(selected))
            {
                nextIndex = (selectable.IndexOf(selected) + (reverse ? -1 : 1)) % selectable.Count;
                if (nextIndex < 0) nextIndex = selectable.Count - 1;
                next = selectable[nextIndex];
            }
            else
                next = selectable[0];

            DoSwitch(next);
        }

        /// <summary>Switches to a character at the specified zero-based index. Returns true if 
        /// the switch was successful, otherwise returns false.</summary>
        public static bool SwitchTo(int index)
        {
            if (!_canSwitchSquadMembers) return false;

            if (index >= 0 && index < _squadMembers.Count)
            {
                IControllable next = _squadMembers[index];
                if (next != selected && next.Controllable)
                {
                    DoSwitch(next);
                    return true;
                }
            }

            return false;
        }
    }
}
