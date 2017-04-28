namespace Dillon.Server.Mappings {
    using Common;

    public class KeyCodesMapping
        : IMapping {

        public static string Name => "keycodes";

        public KeyCodesMapping(IKeyboardSimulatorAdapter keyboard, VirtualKeyCode[] keyCodes) {
            _keyboard = keyboard;
            _keyCodes = keyCodes;
        }

        public void Execute(Update update) {
            _keyboard.KeyPress(_keyCodes);
        }

        private readonly VirtualKeyCode[] _keyCodes;
        private readonly IKeyboardSimulatorAdapter _keyboard;
    }
}