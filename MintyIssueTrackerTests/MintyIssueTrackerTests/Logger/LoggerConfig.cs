using System;
using Serilog;
using Serilog.Events;

namespace MintyIssueTrackerTests.Logger
{
    public static class LoggerConfig
    {
        private static ILogger _logger { get; set; }
        public static ILogger Configure()
        {
            if (_logger == null)
            {
                _logger = new LoggerConfiguration()
                    .WriteTo.File(
                        path: $"{Environment.CurrentDirectory}\\log.log",
                        restrictedToMinimumLevel: LogEventLevel.Information,
                        shared: true)
                    .WriteTo.NUnitOutput()
                    .CreateLogger();
            }
            return _logger;
        }
    }
}
