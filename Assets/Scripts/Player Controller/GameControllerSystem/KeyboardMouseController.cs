using InControl;

namespace JakePerry
{
    public class KeyboardMouseController<T> : GameController<T> where T : GameSpecificInputSet, new()
    {
        public KeyboardMouseController() : base()
        {
            Initialize();
        }

        public KeyboardMouseController(ITarget target) : base(target)
        {
            Initialize();
        }

        private void Initialize()
        {
            type = ControlType.KeyboardAndMouse;

            inputSet.CreateDefaultKeyboardMouse();
        }
    }
}
