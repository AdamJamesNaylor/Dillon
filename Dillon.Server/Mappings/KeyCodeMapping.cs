
using Dillon.Common;

namespace Dillon.Server.Mappings
{
    public class KeyCodeMapping
    {
        public KeyCodeMapping(IKeyboardSimulator keyboard, VirtualKeyCode keyCode)
        {
            _keyboard = keyboard;
        }

        public void Execute()
        {
            _keyboard.KeyPress(_keyCode);
        }

        private VirtualKeyCode _keyCode;
        private IKeyboardSimulator _keyboard;
    }
}
