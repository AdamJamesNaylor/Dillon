namespace Dillon.Server.Configuration {
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Common;
    using JsonConfig;
    using PluginAPI.V1;

    public interface IPluginLoader {
        IMapping Create(dynamic mapping);
        IConfiguration Config { get; set; }
    }

    public class PluginLoader
        : IPluginLoader {

        public PluginLoader(ILoggerAdapter logger) {
            _logger = logger;
            AppDomain.CurrentDomain.AssemblyResolve += LoadFromSameFolder;
        }

        public IMapping Create(dynamic mapping) {
            try {
                var map = mapping.map;
                var type = map.type;
                if (type is NullExceptionPreventer) {
                    throw new Exception($"Mapping with Id {mapping.Id} has an untyped map. All mappings with map properties must specify a type, check the config file.");
                }

                string typeString = type.ToString();
                if (!Directory.Exists("plugins")) {
                    _logger.Warn($"Unable to load mapping with type {typeString} because the plugin directory does not exist.");
                    return null;
                }

                var fileEntries = Directory.GetFiles("plugins");
                foreach (var file in fileEntries) {
                    //app domain approach https://stackoverflow.com/questions/1137781/c-sharp-correct-way-to-load-assembly-find-class-and-call-run-method/14184863#14184863
                    Assembly dll = null;
                    using (var stream = File.OpenRead(file)) {
                        var assemblyData = new byte[stream.Length];
                        stream.Read(assemblyData, 0, assemblyData.Length);
                        try {
                            dll = Assembly.Load(assemblyData);
                        }
                        catch {
                            continue;
                        }
                    }
                    var mappingFactories = dll.GetTypes()
                        .Where(t => t.IsClass && t.GetInterfaces().Contains(typeof (IMappingFactory)));
                    if (!mappingFactories.Any()) {
                        continue;
                    }

                    foreach (var factory in mappingFactories) {
                        var instance = (IMappingFactory) Activator.CreateInstance(factory);
                        instance.RegisterDependancy(_logger);
                        instance.RegisterDependancy(Config);
                        instance.Initiate();
                        if (!instance.SupportedMappings.Contains(typeString))
                            continue;
                        var pluginMapping = instance.Create("joy", map);
                        return pluginMapping;
                    }
                }
            } catch (Exception ex) {
                _logger.Warn($"Exception trying to load plugin mapping for {mapping.id}: " + ex.Message);
                return null;
            }
            return null;
        }

        private static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args) {
            var folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            folderPath = Path.Combine(folderPath, "plugins");
            var assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath))
                return null;

            return Assembly.LoadFrom(assemblyPath);
        }

        private readonly ILoggerAdapter _logger;
        public IConfiguration Config { get; set; }
    }
}