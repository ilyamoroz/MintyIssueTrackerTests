namespace MintyIssueTrackerTests.Logger
{
    public class LoggerHandler
    {
        public delegate void Logger(string message);

        public event Logger Log;
        public LoggerHandler(Logger logger)
        {
            Log += logger;
        }
        protected virtual void OnLog(string message)
        {
            Log(message);

        }
    }
}
