namespace Dillon.PluginAPI.V1 {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PluginFactory
        : IPluginFactory {

        public IEnumerable<string> SupportedPlugins = new List<string> { SoundEffectPlugin.Name };

        public void RegisterDependancy<T>(T dependancy) {
            var type = typeof (T);
            //if (type == typeof(SoundEffectPlugin))

            //_dependancies.Add(dependancy);
        }

        public IPlugin Create(string name) {
            var type = SupportedPlugins.FirstOrDefault(t => t == name);
            if (type == null)
                throw new NotSupportedException($"Plugins of type {name} are not supported by this factory.");

            return new SoundEffectPlugin();
        }
    }

    public interface IPluginFactory
    {
        void RegisterDependancy<T>(T dependancy);
    }

    public interface IPlugin
    {
    }

    public class SoundEffectPlugin
        : IPlugin
    {
        public static string Name => "sfx";
    }

}