namespace Dillon.Server.Mappings {
    using Common;
    using Controllers;
    using PluginAPI.V1;

    public class TextEntryMapping
        : IMapping {
        public TextEntryMapping(IKeyboardSimulatorAdapter keyboard, string text) {
            _keyboard = keyboard;
            _text = text;
        }

        public void Execute(Update update) {
            _keyboard.TextEntry(_text);
        }

        private readonly string _text;
        private readonly IKeyboardSimulatorAdapter _keyboard;
    }
}