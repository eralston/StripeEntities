using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using System;

namespace StripeEntities
{
    public class Logger
    {
        public static void Log<T>(string message, LogLevel level = LogLevel.Information, params object[] args)
        {
            var loggerFactory = new LoggerFactory()
                .AddConsole();
            var logger = loggerFactory.CreateLogger<T>();
            logger.Log<object>(level, (EventId)0, (object)new FormattedLogValues(message, args), (Exception)null, (state, error) => state.ToString());
        }
    }
}