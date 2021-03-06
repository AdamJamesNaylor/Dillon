namespace Dillon.Server.Configuration {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;
    using Common;
    using Diagnostics;
    using JsonConfig;
    using Mappings;
    using NLog;

    public interface IConfigurator {
        Configuration Configure(string[] args);
    }

    public class Configurator
        : IConfigurator {

        public Configurator(ICoreMappingFactory coreMappingFactory, IPluginLoader pluginLoader) {
            _coreMappingFactory = coreMappingFactory;
            _pluginLoader = pluginLoader;
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
            _pluginLoader.Config = config;

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
                else {
                    var pluginMapping = _pluginLoader.Create(mapping);
                    if (pluginMapping != null)
                        config.Mappings.Add(mapping.id, pluginMapping);

                    //todo check for nullmapping and log warning if no factories have handled it
                }
            }
        }

        private readonly ICoreMappingFactory _coreMappingFactory;
        private readonly IPluginLoader _pluginLoader;
    }
}