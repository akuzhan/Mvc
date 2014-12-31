using System;
using System.Collections.Generic;

namespace LoggingWebSite
{
    public class CustomSink
    {
        public List<LogEntry> LogEntries { get; set; } = new List<LogEntry>();
        
    }
}