﻿namespace Dillon.Server.Mappings {
    using Common;

    public class MouseVerticalScrollMapping
        : IMapping {

        public static string Name => "vscroll";

        public MouseVerticalScrollMapping(IMouseSimulatorAdapter mouseSimulator) {
            _mouseSimulator = mouseSimulator;
        }

        public void Execute(Update update) {
            var delta = update.Y - _position;
            _mouseSimulator.VerticalScroll((int)delta);
            _position = update.Y;
        }

        private double _position;
        private readonly IMouseSimulatorAdapter _mouseSimulator;
    }
}