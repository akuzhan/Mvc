using System;
using Microsoft.Framework.Logging;

namespace LoggingWebSite
{
    public class CustomLogger : ILogger
    {
        private readonly string _loggerName;
        private readonly CustomSink _sink;

        public CustomLogger(string loggerName, CustomSink sink)
        {
            _loggerName = loggerName;
            _sink = sink;
        }

        public IDisposable BeginScope(object state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true; //todo: fix this later
        }

        public void Write(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            _sink.LogEntries.Add(new LogEntry()
            {
                EventId = eventId,
                State = state,
                Exception = exception,
                LogLevel = logLevel
            });
        }
    }
}