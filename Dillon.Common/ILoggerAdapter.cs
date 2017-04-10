
namespace Dillon.Common {
    public interface ILoggerAdapter {
        void Trace(string message);
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error<T>(T value);
        void Fatal(string message);
    }
}