namespace Dillon.Server.Mappings {
    using Common;
    using PluginAPI.V1;

    public class MouseHorizontalScrollMapping : IMapping
    {
        public MouseHorizontalScrollMapping(IMouseSimulatorAdapter mouseSimulator) {
            _mouseSimulator = mouseSimulator;
        }

        public void Execute(Update update) {
            var delta = update.Y - _position;
            _mouseSimulator.HorizontalScroll((int)delta);
            _position = update.X;
        }

        private double _position;
        private readonly IMouseSimulatorAdapter _mouseSimulator;
    }
}