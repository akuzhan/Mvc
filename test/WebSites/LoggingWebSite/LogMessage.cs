﻿using System;
using Microsoft.Framework.Logging;

namespace LoggingWebSite
{
    public class LogMessage
    {
        public int EventID { get; set; }

        public string Message { get; set; }

        public string LoggerName { get; set; }

        public LogLevel Severity { get; set; }

        public object State { get; set; }

        public DateTimeOffset Time { get; set; }

        public HttpRequestInfo RequestInfo { get; set; }
    }
}