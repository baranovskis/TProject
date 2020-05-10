using NLog;

namespace TPClient.Utilities
{
    internal static class Log
    {
        public static Logger Instance { get; }

        static Log()
        {
            LogManager.ReconfigExistingLoggers();
            Instance = LogManager.GetCurrentClassLogger();
        }
    }
}
