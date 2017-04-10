
namespace Dillon.PluginAPI {
    using Common;
    using V1;

    public class NullMapping
        : IMapping {
        public void Execute(Update update) { }
    }
}