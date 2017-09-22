using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarForceRecon
{
    public static class SquadManager
    {
        #region Switch event, IControllable interface

        public interface IControllable
        {
            void OnSwitchTo();
            void OnSwitchAway();

            UnityEngine.Transform transform { get; }
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
        private static int _selectedIndex = -1;
        private static IControllable _selected = null;

        public static List<IControllable> GetSquadMembers { get { return _squadMembers; } }

        public static IControllable GetCurrentSquaddie { get { return _selected; } }

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

                // If no characters are selected
                if (_selected == null)
                {
                    _selectedIndex = _squadMembers.Count - 1;
                    _selected = member;

                    member.OnSwitchTo();
                    RaiseSwitchEvent();
                }
                else
                    member.OnSwitchAway();
            }
        }

        /// <summary>Switches to the next available character. Will iterate backwards if reverse is true.</summary>
        public static void Switch(bool reverse = false)
        {
            if (!_canSwitchSquadMembers) return;

            if (_squadMembers.Count == 0)
            {
                Debug.LogWarning("Warning: No current squad members. Cannot switch.");
                return;
            }

            if (_squadMembers.Count == 1)
            {
                // Only one squad member available
                _selected = _squadMembers[0];
                _selectedIndex = 0;
                return;
            }

            // Get index of the next squad member
            int previousSelected = _selectedIndex;
            int finalIndex = _squadMembers.Count - 1;
            int next = _selectedIndex;

            if (!reverse)
            {
                if (next >= finalIndex)
                    next = 0;
                else
                    next++;
            }
            else
            {
                if (next == 0)
                    next = finalIndex;
                else
                    next--;
            }

            // Select new squad member
            _selectedIndex = next;
            _selected = _squadMembers[_selectedIndex];

            // Trigger switch event
            _squadMembers[previousSelected].OnSwitchAway();
            _selected.OnSwitchTo();
            RaiseSwitchEvent();
        }

        /// <summary>Switches to a character at the specified zero-based index. Returns true if 
        /// the switch was successful, otherwise returns false.</summary>
        public static bool SwitchTo(int index)
        {
            if (!_canSwitchSquadMembers)
                return false;

            if (index >= 0 && index < _squadMembers.Count)
            {
                if (index != _selectedIndex)
                {
                    _selected.OnSwitchAway();

                    _selectedIndex = index;
                    _selected = _squadMembers[index];

                    _selected.OnSwitchTo();
                    RaiseSwitchEvent();

                    return true;
                }
            }

            return false;
        }
    }
}
