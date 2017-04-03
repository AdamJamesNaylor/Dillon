namespace Dillon.Server {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using JsonConfig;
    using NLog;

    public interface IConfiguration {
        bool Debug { get; }
        string UIFolder { get; }
        string UI { get; }
        string Scheme { get; }
        string Domain { get; }
        int Port { get; }
        IDictionary<int, int> Mappings { get; set; }
    }

    public class Configuration
        : IConfiguration {

        public static Configuration Empty => new Configuration();

        public bool Debug { get; set; }

        public string UIFolder{ get; set; }

        public string UI { get; set; }

        public string Scheme { get; private set; }
        public string Domain { get; private set; }
        public int Port { get; private set; }

        public IDictionary<int, int> Mappings { get; set; }

        public static void Parse(Configuration config) {
            var log = LogManager.GetCurrentClassLogger();

            var uiFolder = Config.Global.uiFolder;
            if (!(uiFolder is NullExceptionPreventer)) {
                config.UIFolder = uiFolder.ToString().TrimEnd(Path.DirectorySeparatorChar);
            }
            config.UI = Config.Global.ui;
            config.Scheme = Config.Global.scheme;
            config.Domain = Config.Global.domain;
            config.Port = Config.Global.port;

            config.Mappings = new Dictionary<int, int>();

            var mappings = Config.Global.mappings;
            if (mappings is NullExceptionPreventer) {
                log.Warn("No mappings found in configuration file.");
                return;
            }

            foreach (var mapping in mappings) {
                //string type = mapping.type.ToString();
                //switch (type) {
                    //todo add a plugin mapping factory that gives each mapping a chance to be created with the correct dependancies.
                //}
                config.Mappings.Add(mapping.id, mapping.keyCode);
            }
        }
    }

    public abstract class Mapping {
        public int Id { get; set; }

        public abstract void Execute();
    }

    public class Binding
        : Mapping {
        public int KeyCode { get; set; }

        public override void Execute() {
            throw new NotImplementedException();
        }
    }

    public class Macro 
        : Mapping {
        public IEnumerable<int> KeyCodes { get; set; }

        public override void Execute() {
            throw new NotImplementedException();
        }
    }

    //todo plugins
    /*
     during app startup the plugin directory is scanned.
     Each plugin found should have a startup/registry class that get's registered with the app's startup code
     during configuration this code is then used to create each mapping that is found for that plugin
     - this means I'll need a way to query something by mapping type which in turns finds the right factory for that type
     this code then returns a mapping object created with the right dependancies
     */

}