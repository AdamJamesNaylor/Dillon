namespace Dillon.Server.Mappings {
    using Common;
    using Controllers;
    using PluginAPI.V1;

    public class KeyCodesMapping
        : IMapping {
        public KeyCodesMapping(IKeyboardSimulator keyboard, VirtualKeyCode[] keyCodes) {
            _keyboard = keyboard;
            _keyCodes = keyCodes;
        }

        public void Execute(Update update) {
            _keyboard.KeyPress(_keyCodes);
        }

        private readonly VirtualKeyCode[] _keyCodes;
        private readonly IKeyboardSimulator _keyboard;
    }
}