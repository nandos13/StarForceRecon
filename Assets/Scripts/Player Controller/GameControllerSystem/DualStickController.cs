using InControl;

namespace JakePerry
{
    public class DualStickController<T> : GameController<T> where T : GameSpecificInputSet, new()
    {
        public DualStickController() : base()
        {
            Initialize();
        }

        public DualStickController(ITarget target) : base(target)
        {
            Initialize();
        }

        private void Initialize()
        {
            type = ControlType.Gamepad;

            inputSet.CreateDefaultDualStick();
        }
    }
}
