
namespace Dillon.Common {
    using System.Collections.Generic;

    public interface IConfiguration {
        bool Debug { get; }
        string UIFolder { get; }
        string UI { get; }
        string Scheme { get; }
        string Domain { get; }
        int Port { get; }
        int ButtonDelay { get; }
        IDictionary<int, IMapping> Mappings { get; set; }
    }
}