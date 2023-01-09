using Microsoft.Extensions.Logging;

namespace MassTransitDemo
{
    public static class Logger
    {
        private static ILogger _logger;

        public static void Set(ILogger logger)
        {
            _logger = logger;
        }

        public static void Log(string message)
        {
            _logger?.LogInformation(message);
        }
    }
}
