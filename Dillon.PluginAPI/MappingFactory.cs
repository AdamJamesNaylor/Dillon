namespace Dillon.PluginAPI.V1 {
    using Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PlaySoundEffectMappingFactory
        : IMappingFactory {

        public IEnumerable<string> SupportedMappings = new List<string> { PlaySoundEffectMapping.Name };

        public void RegisterDependancy<T>(T dependancy) {
            if (dependancy is IKeyboardSimulator) {
                _keyboardSimulator = (IKeyboardSimulator)dependancy;
            }
        }

        public IMapping Create(string name) {
            var type = SupportedMappings.FirstOrDefault(t => t == name);
            if (type == null)
                throw new NotSupportedException($"Mappings of type {name} are not supported by this factory.");

            if (_keyboardSimulator == null)
                throw new InvalidOperationException("No reference to a IKeyboardSimulator was registered.");

            return new PlaySoundEffectMapping();
        }

        private IKeyboardSimulator _keyboardSimulator = null;
    }

    public interface IMappingFactory
    {
        void RegisterDependancy<T>(T dependancy);
        IMapping Create(string name);
    }

    public interface IMapping
    {
    }

    public class PlaySoundEffectMapping
        : IMapping {

        public static string Name => "sfx";

        public PlaySoundEffectMapping()
        {

        }
    }

}