using UnityEngine;

namespace EntryPoint
{
    public class GameLogger : IGameLoggerService
    {
        public LogType LogType { get; }

        public GameLogger(LogType logType)
        {
            LogType = logType;
        }
        
        void IGameLoggerService.Log(LogType logType, string message)
        {
            if (LogType == LogType.All || LogType == logType)
            {
                Debug.Log(message);
            }
        }
    }
}