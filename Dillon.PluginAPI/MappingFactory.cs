namespace Dillon.PluginAPI.V1 {
    using Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PlaySoundEffectMappingFactory
        : IMappingFactory {

        public IEnumerable<string> SupportedMappings => new List<string> {PlaySoundEffectMapping.Name};

        public void Initiate() {
            
        }

        public void RegisterDependancy<T>(T dependancy) {
            var simulator = dependancy as IKeyboardSimulatorAdapter;
            if (simulator != null) {
                _keyboardSimulator = simulator;
            }
        }

        public IMapping Create(string name, IDictionary<string, object> mapping) {
            var type = SupportedMappings.FirstOrDefault(t => t == name);
            if (type == null)
                throw new NotSupportedException($"Mappings of type {name} are not supported by this factory.");

            if (_keyboardSimulator == null)
                throw new InvalidOperationException("No reference to a IKeyboardSimulator was registered.");

            return new PlaySoundEffectMapping();
        }

        private IKeyboardSimulatorAdapter _keyboardSimulator = null;
    }

    public interface IMappingFactory {
        IEnumerable<string> SupportedMappings { get; }
        void Initiate();
        void RegisterDependancy<T>(T dependancy);
        IMapping Create(string name, IDictionary<string, object> map);
    }

    public class PlaySoundEffectMapping
        : IMapping {

        public static string Name => "sfx";

        public PlaySoundEffectMapping() {

        }

        public void Execute(Update update) {
            throw new NotImplementedException();
        }
    }

}