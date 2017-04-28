namespace Dillon.Server.Configuration {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;
    using Common;
    using Diagnostics;
    using JsonConfig;
    using Mappings;
    using NLog;
    using PluginAPI.V1;

    public interface IConfigurator {
        Configuration Configure(string[] args);
    }

    public class Configurator
        : IConfigurator {

        public Configurator(ICoreMappingFactory coreMappingFactory) {
            _coreMappingFactory = coreMappingFactory;

            AppDomain.CurrentDomain.AssemblyResolve += LoadFromSameFolder;
        }

        public Configuration Configure(string[] args) {
            Configuration config = null;

            try {
                config = new Configuration();
                ApplyCommandArguments(args, config);
                Parse(config);

            } catch (TypeInitializationException e) {
                throw new Exception($"Exception parsing configuration file(s). {e.InnerMostException().Message}", e);
            }

            return config;
        }

        private void ApplyCommandArguments(string[] args, Server.Configuration.Configuration config) {
            foreach (var arg in args) {
                if (arg == "-d")
                    config.Debug = true;
            }

#if DEBUG
            config.Debug = true;
#endif
            config.UIFolder = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "UI");

            if (config.Debug)
                ApplyDebugMode();
        }

        private void ApplyDebugMode() {
            //set log level to TRACE
            //set archiveOnStartup to true
        }

        private void Parse(Configuration config) {
            var log = LogManager.GetCurrentClassLogger();

            var uiFolder = Config.Global.uiFolder;
            if (!(uiFolder is NullExceptionPreventer)) {
                config.UIFolder = uiFolder.ToString().TrimEnd(Path.DirectorySeparatorChar);
            }
            config.UI = Config.Global.ui;
            config.Scheme = Config.Global.scheme;
            config.Domain = Config.Global.domain;
            config.Port = Config.Global.port;

            config.ButtonDelay = Config.Global.buttonDelay;

            config.Mappings = new Dictionary<int, IMapping>();

            var mappings = Config.Global.mappings;
            if (mappings is NullExceptionPreventer) {
                log.Warn("No mappings found in configuration file.");
                return;
            }

            //todo add a plugin mapping factory that gives each mapping a chance to be created with the correct dependancies.
            //todo temp. will be replaced with proper plugin loading code
            var logAdapter = new LoggerAdapter(log);

            foreach (var mapping in mappings) {
                var map = mapping.map;
                if (map is NullExceptionPreventer) { //core mapping
                    var keyCode = mapping.keyCode;
                    //todo remove string from factory and make factories determin if they can create something or not based on what gets passed in
                    if (!(keyCode is NullExceptionPreventer)) {
                        config.Mappings.Add(mapping.id, _coreMappingFactory.Create("keycode", mapping));
                    }

                    var text = mapping.text;
                    if (!(text is NullExceptionPreventer)) {
                        config.Mappings.Add(mapping.id, _coreMappingFactory.Create("textentry", mapping));
                    }

                    var keycodes = mapping.keyCodes;
                    if (!(keycodes is NullExceptionPreventer)) {
                        config.Mappings.Add(mapping.id, _coreMappingFactory.Create("keycodes", mapping));
                    }

                    var coreType = mapping.type;
                    if (!(coreType is NullExceptionPreventer)) {
                        if (coreType == "vscroll" || coreType == "hscroll")
                            config.Mappings.Add(mapping.id, _coreMappingFactory.Create(coreType, mapping));
                    }
                }
                else { //plugin mapping
                    var type = map.type;
                    if (type is NullExceptionPreventer) {
                        throw new Exception($"Mapping with Id {mapping.Id} has an untyped map. All mappings with map properties must specify a type, check the config file.");
                    }

                    string typeString = type.ToString();
                    if (!Directory.Exists("plugins")) {
                        log.Warn($"Unable to load mapping with type {typeString} because the plugin directory does not exist.");
                        continue;
                    }

                    var fileEntries = Directory.GetFiles("plugins");
                    foreach (var file in fileEntries) {
                        Assembly dll = null;
                        using (var stream = File.OpenRead(file)) {
                            var assemblyData = new byte[stream.Length];
                            stream.Read(assemblyData, 0, assemblyData.Length);
                            try {
                                dll = Assembly.Load(assemblyData);
                            }
                            catch {
                                //will replace this with http://stackoverflow.com/a/14184863/17540
                                continue;
                            }
                        }
                        var mappingFactories = dll.GetTypes()
                            .Where(t => t.IsClass && t.GetInterfaces().Contains(typeof (IMappingFactory)));
                        if (!mappingFactories.Any()) {
                            continue;
                        }

                        foreach (var factory in mappingFactories) {
                            var instance = (IMappingFactory)Activator.CreateInstance(factory);
                            instance.RegisterDependancy(logAdapter);
                            instance.RegisterDependancy(config);
                            instance.Initiate();
                            if (!instance.SupportedMappings.Contains(typeString))
                                continue;
                            var pluginMapping = instance.Create("joy", map);
                            config.Mappings.Add(mapping.id, pluginMapping);
                        }
                    }
                    //todo check for nullmapping and log warning if no factories have handled it
                }
            }
        }

        private readonly ICoreMappingFactory _coreMappingFactory;

        private static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
        {
            string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            folderPath = Path.Combine(folderPath, "plugins");
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath)) return null;
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }
    }

    
}