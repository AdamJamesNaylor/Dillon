
namespace Dillon.Server.Diagnostics {
    using Common;
    using NLog;

    internal class LoggerAdapter
        : ILoggerAdapter {

        public LoggerAdapter(ILogger logger) {
            _logger = logger;
        }

        public void Trace(string message) {
            _logger.Trace(message);
        }

        public void Debug(string message) {
            _logger.Debug(message);
        }

        public void Info(string message) {
            _logger.Info(message);
        }

        public void Warn(string message) {
            _logger.Warn(message);
        }

        public void Error<T>(T value) {
            _logger.Error(value);
        }

        public void Fatal(string message) {
            _logger.Fatal(message);
        }

        private readonly ILogger _logger;
    }
}