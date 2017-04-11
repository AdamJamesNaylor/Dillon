namespace Dillon.Server {
    using System;
    using System.Collections.Generic;
    using PluginAPI.V1;

    public interface IConfiguration {
        bool Debug { get; }
        string UIFolder { get; }
        string UI { get; }
        string Scheme { get; }
        string Domain { get; }
        int Port { get; }
        IDictionary<int, IMapping> Mappings { get; set; }
    }

    public class Configuration
        : IConfiguration {

        public static Configuration Empty => new Configuration();

        public bool Debug { get; set; }

        public string UIFolder{ get; set; }

        public string UI { get; set; }

        public string Scheme { get; set; }
        public string Domain { get; set; }
        public int Port { get; set; }

        public IDictionary<int, IMapping> Mappings { get; set; }

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