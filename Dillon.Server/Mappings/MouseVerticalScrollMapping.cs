namespace Dillon.Server.Mappings {
    using Common;
    using PluginAPI.V1;

    public class MouseVerticalScrollMapping
        : IMapping {

        public MouseVerticalScrollMapping(IMouseSimulator mouseSimulator) {
            _mouseSimulator = mouseSimulator;
        }

        public void Execute(Update update) {
            var delta = update.Y - _position;
            _mouseSimulator.VerticalScroll((int)delta);
            _position = update.Y;
        }

        private double _position;
        private readonly IMouseSimulator _mouseSimulator;
    }
}