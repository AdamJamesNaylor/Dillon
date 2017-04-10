
using Dillon.Common;

namespace Dillon.Server.Mappings {
    using Controllers;
    using PluginAPI.V1;

    public class KeyCodeMapping
        : IMapping {
        public KeyCodeMapping(IKeyboardSimulator keyboard, VirtualKeyCode keyCode) {
            _keyboard = keyboard;
            _keyCode = keyCode;
        }

        public void Execute(Update update) {
            _keyboard.KeyPress(_keyCode);
        }

        private readonly VirtualKeyCode _keyCode;
        private readonly IKeyboardSimulator _keyboard;
    }
}
