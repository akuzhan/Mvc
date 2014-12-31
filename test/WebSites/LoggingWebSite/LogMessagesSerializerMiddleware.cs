using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.Framework.DependencyInjection;

namespace LoggingWebSite
{
    public class LogMessagesSerializerMiddleware
    {
        private readonly RequestDelegate _nextMiddleware;

        public LogMessagesSerializerMiddleware(RequestDelegate nextMiddleware)
        {
            _nextMiddleware = nextMiddleware;
        }

        public async Task Invoke(HttpContext context)
        {
            // get log messages from the sink
            var customSink = context.RequestServices.GetService<CustomSink>();

            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";

            using (var sw = new StreamWriter(context.Response.Body, Encoding.UTF8, 1024, leaveOpen: true))
            {
                var data = JsonConvert.SerializeObject(customSink.LogEntries, Formatting.Indented, new StringEnumConverter());

                await sw.WriteAsync(data);
            }
        }
    }
}