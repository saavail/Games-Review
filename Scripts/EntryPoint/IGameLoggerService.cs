using DependencyInjector;

namespace EntryPoint
{
    public interface IGameLoggerService : IService
    {
        public LogType LogType { get; }
        void Log(LogType logType, string message);
    }
}