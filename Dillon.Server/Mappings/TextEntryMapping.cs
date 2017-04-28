namespace Dillon.Server.Mappings {
    using Common;

    public class TextEntryMapping
        : IMapping {

        public static string Name => "textentry";

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