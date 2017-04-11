
namespace Dillon.Server.Input {
    using Common;

    public class MouseSimulatorAdapter
        : IMouseSimulatorAdapter {

        public MouseSimulatorAdapter(WindowsInput.IMouseSimulator mouseSimulator) {
            _mouseSimulator = mouseSimulator;
        }

        public IMouseSimulatorAdapter MoveMouseBy(int pixelDeltaX, int pixelDeltaY) {
            _mouseSimulator.MoveMouseBy(pixelDeltaX, pixelDeltaY);
            return this;
        }

        public IMouseSimulatorAdapter MoveMouseTo(double absoluteX, double absoluteY) {
            _mouseSimulator.MoveMouseTo(absoluteX, absoluteY);
            return this;
        }

        public IMouseSimulatorAdapter LeftButtonDown() {
            _mouseSimulator.LeftButtonDown();
            return this;
        }

        public IMouseSimulatorAdapter LeftButtonUp() {
            _mouseSimulator.LeftButtonUp();
            return this;
        }

        public IMouseSimulatorAdapter LeftButtonClick() {
            _mouseSimulator.LeftButtonClick();
            return this;
        }

        public IMouseSimulatorAdapter LeftButtonDoubleClick() {
            _mouseSimulator.LeftButtonDoubleClick();
            return this;
        }

        public IMouseSimulatorAdapter RightButtonDown() {
            _mouseSimulator.RightButtonDown();
            return this;
        }

        public IMouseSimulatorAdapter RightButtonUp() {
            _mouseSimulator.RightButtonUp();
            return this;
        }

        public IMouseSimulatorAdapter RightButtonClick() {
            _mouseSimulator.RightButtonClick();
            return this;
        }

        public IMouseSimulatorAdapter RightButtonDoubleClick() {
            _mouseSimulator.RightButtonDoubleClick();
            return this;
        }

        public IMouseSimulatorAdapter XButtonDown(int buttonId) {
            _mouseSimulator.XButtonDown(buttonId);
            return this;
        }

        public IMouseSimulatorAdapter XButtonUp(int buttonId) {
            _mouseSimulator.XButtonUp(buttonId);
            return this;
        }

        public IMouseSimulatorAdapter XButtonClick(int buttonId) {
            _mouseSimulator.XButtonClick(buttonId);
            return this;
        }

        public IMouseSimulatorAdapter XButtonDoubleClick(int buttonId) {
            _mouseSimulator.XButtonDoubleClick(buttonId);
            return this;
        }

        public IMouseSimulatorAdapter VerticalScroll(int scrollAmountInClicks) {
            _mouseSimulator.VerticalScroll(scrollAmountInClicks);
            return this;
        }

        public IMouseSimulatorAdapter HorizontalScroll(int scrollAmountInClicks) {
            _mouseSimulator.HorizontalScroll(scrollAmountInClicks);
            return this;
        }

        private readonly WindowsInput.IMouseSimulator _mouseSimulator;
    }
}