using System;
using Microsoft.Framework.Logging;

namespace LoggingWebSite
{
    public class LogEntry
    {
        public LogLevel LogLevel { get; set; }

        public int EventId { get; set; }

        public object State { get; set; }

        public Exception Exception { get; set; }
    }
}