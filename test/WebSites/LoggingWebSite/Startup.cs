// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;

namespace LoggingWebSite
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            var configuration = app.GetTestConfiguration();

            // Set up application services
            app.UseServices(services =>
            {
                services.AddSingleton<ILoggerFactory, CustomLoggerFactory>();

                services.AddSingleton<CustomSink>();

                // Add MVC services to the services container
                services.AddMvc(configuration);
            });

            // serializes log messages from TestSink into json format to the client
            app.Map(new PathString("/log"), (appBuilder) =>
            {
                appBuilder.UseMiddleware<LogMessagesSerializerMiddleware>();
            });

            // Add MVC to the request pipeline
            app.UseMvc(routes =>
            {
                routes.MapRoute("Default", "{controller=Home}/{action=Index}");
            });
        }
    }
}
