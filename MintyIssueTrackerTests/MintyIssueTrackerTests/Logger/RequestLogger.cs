using Serilog;

namespace MintyIssueTrackerTests.Logger
{
    public class RequestLogger : LoggerHandler
    {
        private static ILogger logger = LoggerConfig.Configure();

        public RequestLogger() : base(CreateLog){ }


        private static void CreateLog(string message)
        {
            logger.Information(message);
        }

        /// <summary>
        /// Write log
        /// </summary>
        public void WriteToLog(string message)
        {
            OnLog(message);
        }
    }
}
