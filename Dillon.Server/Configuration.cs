namespace Dillon.Server {
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
                config.Mappings.Add(mapping.id, mapping.keyCode);
            }
        }
    }

    

}