
namespace Dillon.Server.Input {
    using Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class KeyboardSimulatorAdapter
        : IKeyboardSimulator {

        public KeyboardSimulatorAdapter(WindowsInput.IKeyboardSimulator inputSimulator) {
            _inputSimulator = inputSimulator;
        }

        public IKeyboardSimulator KeyDown(VirtualKeyCode keyCode) {
            _inputSimulator.KeyDown((WindowsInput.Native.VirtualKeyCode) keyCode);
            return this;
        }

        public IKeyboardSimulator KeyPress(params VirtualKeyCode[] keyCodes) {
            _inputSimulator.KeyPress(Array.ConvertAll(keyCodes, i => (WindowsInput.Native.VirtualKeyCode) i));
            return this;
        }

        public IKeyboardSimulator KeyPress(VirtualKeyCode keyCode) {
            _inputSimulator.KeyPress((WindowsInput.Native.VirtualKeyCode) keyCode);
            return this;
        }

        public IKeyboardSimulator KeyUp(VirtualKeyCode keyCode) {
            _inputSimulator.KeyUp((WindowsInput.Native.VirtualKeyCode) keyCode);
            return this;
        }

        public IKeyboardSimulator ModifiedKeyStroke(VirtualKeyCode modifierKey, IEnumerable<VirtualKeyCode> keyCodes) {
            _inputSimulator.ModifiedKeyStroke((WindowsInput.Native.VirtualKeyCode) modifierKey,
                keyCodes.Cast<WindowsInput.Native.VirtualKeyCode>());
            return this;
        }

        public IKeyboardSimulator ModifiedKeyStroke(VirtualKeyCode modifierKeyCode, VirtualKeyCode keyCode) {
            _inputSimulator.ModifiedKeyStroke((WindowsInput.Native.VirtualKeyCode) modifierKeyCode,
                (WindowsInput.Native.VirtualKeyCode) keyCode);
            return this;
        }

        public IKeyboardSimulator ModifiedKeyStroke(IEnumerable<VirtualKeyCode> modifierKeyCodes, VirtualKeyCode keyCode) {
            _inputSimulator.ModifiedKeyStroke(modifierKeyCodes.Cast<WindowsInput.Native.VirtualKeyCode>(),
                (WindowsInput.Native.VirtualKeyCode) keyCode);
            return this;
        }

        public IKeyboardSimulator ModifiedKeyStroke(IEnumerable<VirtualKeyCode> modifierKeyCodes,
            IEnumerable<VirtualKeyCode> keyCodes) {
            _inputSimulator.ModifiedKeyStroke(modifierKeyCodes.Cast<WindowsInput.Native.VirtualKeyCode>(),
                keyCodes.Cast<WindowsInput.Native.VirtualKeyCode>());
            return this;
        }

        public IKeyboardSimulator Sleep(TimeSpan timeout) {
            _inputSimulator.Sleep(timeout);
            return this;
        }

        public IKeyboardSimulator Sleep(int millsecondsTimeout) {
            _inputSimulator.Sleep(millsecondsTimeout);
            return this;
        }

        public IKeyboardSimulator TextEntry(char character) {
            _inputSimulator.TextEntry(character);
            return this;
        }

        public IKeyboardSimulator TextEntry(string text) {
            _inputSimulator.TextEntry(text);
            return this;
        }

        private readonly WindowsInput.IKeyboardSimulator _inputSimulator;
    }
}