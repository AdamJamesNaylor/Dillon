
namespace Dillon.Server.Input {
    using Common;

    public class MouseSimulatorAdapter
        : IMouseSimulator {

        public MouseSimulatorAdapter(WindowsInput.IMouseSimulator mouseSimulator) {
            _mouseSimulator = mouseSimulator;
        }

        public IMouseSimulator MoveMouseBy(int pixelDeltaX, int pixelDeltaY) {
            _mouseSimulator.MoveMouseBy(pixelDeltaX, pixelDeltaY);
            return this;
        }

        public IMouseSimulator MoveMouseTo(double absoluteX, double absoluteY) {
            _mouseSimulator.MoveMouseTo(absoluteX, absoluteY);
            return this;
        }

        public IMouseSimulator LeftButtonDown() {
            _mouseSimulator.LeftButtonDown();
            return this;
        }

        public IMouseSimulator LeftButtonUp() {
            _mouseSimulator.LeftButtonUp();
            return this;
        }

        public IMouseSimulator LeftButtonClick() {
            _mouseSimulator.LeftButtonClick();
            return this;
        }

        public IMouseSimulator LeftButtonDoubleClick() {
            _mouseSimulator.LeftButtonDoubleClick();
            return this;
        }

        public IMouseSimulator RightButtonDown() {
            _mouseSimulator.RightButtonDown();
            return this;
        }

        public IMouseSimulator RightButtonUp() {
            _mouseSimulator.RightButtonUp();
            return this;
        }

        public IMouseSimulator RightButtonClick() {
            _mouseSimulator.RightButtonClick();
            return this;
        }

        public IMouseSimulator RightButtonDoubleClick() {
            _mouseSimulator.RightButtonDoubleClick();
            return this;
        }

        public IMouseSimulator XButtonDown(int buttonId) {
            _mouseSimulator.XButtonDown(buttonId);
            return this;
        }

        public IMouseSimulator XButtonUp(int buttonId) {
            _mouseSimulator.XButtonUp(buttonId);
            return this;
        }

        public IMouseSimulator XButtonClick(int buttonId) {
            _mouseSimulator.XButtonClick(buttonId);
            return this;
        }

        public IMouseSimulator XButtonDoubleClick(int buttonId) {
            _mouseSimulator.XButtonDoubleClick(buttonId);
            return this;
        }

        public IMouseSimulator VerticalScroll(int scrollAmountInClicks) {
            _mouseSimulator.VerticalScroll(scrollAmountInClicks);
            return this;
        }

        public IMouseSimulator HorizontalScroll(int scrollAmountInClicks) {
            _mouseSimulator.HorizontalScroll(scrollAmountInClicks);
            return this;
        }

        private readonly WindowsInput.IMouseSimulator _mouseSimulator;
    }
}