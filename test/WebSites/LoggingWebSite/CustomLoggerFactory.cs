using System;
using Microsoft.Framework.Logging;

namespace LoggingWebSite
{
    public class CustomLoggerFactory : ILoggerFactory
    {
        private CustomSink _sink;

        public CustomLoggerFactory(CustomSink sink)
        {
            _sink = sink;
        }

        public ILogger Create(string name)
        {
            return new CustomLogger(name, _sink);
        }

        public void AddProvider(ILoggerProvider provider)
        {
            throw new NotImplementedException();
        }
    }
}