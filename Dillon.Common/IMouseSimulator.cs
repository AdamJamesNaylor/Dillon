namespace Dillon.Common {
    public interface IMouseSimulatorAdapter
    {
        IMouseSimulatorAdapter HorizontalScroll(int scrollAmountInClicks);
        IMouseSimulatorAdapter LeftButtonClick();
        IMouseSimulatorAdapter LeftButtonDoubleClick();
        IMouseSimulatorAdapter LeftButtonDown();
        IMouseSimulatorAdapter LeftButtonUp();
        IMouseSimulatorAdapter MoveMouseBy(int pixelDeltaX, int pixelDeltaY);
        IMouseSimulatorAdapter MoveMouseTo(double absoluteX, double absoluteY);
        IMouseSimulatorAdapter RightButtonClick();
        IMouseSimulatorAdapter RightButtonDoubleClick();
        IMouseSimulatorAdapter RightButtonDown();
        IMouseSimulatorAdapter RightButtonUp();
        IMouseSimulatorAdapter VerticalScroll(int scrollAmountInClicks);
        IMouseSimulatorAdapter XButtonClick(int buttonId);
        IMouseSimulatorAdapter XButtonDoubleClick(int buttonId);
        IMouseSimulatorAdapter XButtonDown(int buttonId);
        IMouseSimulatorAdapter XButtonUp(int buttonId);
    }
}