namespace Dillon.Server.Mappings
{
    using System;
    using System.Collections.Generic;
    using Common;
    using PluginAPI.V1;

    public class CoreMappingFactory
        : IMappingFactory {

        public void RegisterDependancy<T>(T dependancy) {
            var simulator = dependancy as IKeyboardSimulator;
            if (simulator != null) {
                _keyboardSimulator = simulator;
            }
        }

        public IMapping Create(string name, IDictionary<string, object> mapping) {
            switch (name.ToLower()) {
                case "keycode":
                    return new KeyCodeMapping(_keyboardSimulator, ConvertToKeyCode(Convert.ToInt32(mapping["keyCode"])));
                case "textentry":
                    return new TextEntryMapping(_keyboardSimulator, mapping["text"].ToString());
                case "keycodes":
                    return new KeyCodesMapping(_keyboardSimulator, (VirtualKeyCode[])mapping["keyCodes"]);
            }

            throw new NotSupportedException($"'{name}' mappings are not supported by the core mapping factory.");
        }

        private VirtualKeyCode ConvertToKeyCode(int value) {
            return (VirtualKeyCode)value;
        }


        private IKeyboardSimulator _keyboardSimulator;
    }
}
