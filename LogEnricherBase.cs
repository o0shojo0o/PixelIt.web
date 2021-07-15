using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;
using System;

namespace PixelIT.web
{
    public class LogEnricherBase : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("Application", App.ApplicationName));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("MachineName", Environment.MachineName.ToLower()));

            // Log für Webanwendung
            var httpContext = new HttpContextAccessor().HttpContext;
            if (httpContext != null)
            {
                var request = httpContext.Request;
                var rawURL = $"{request.HttpContext.Request.Scheme}://{request.HttpContext.Request.Host}{request.HttpContext.Request.Path}{request.HttpContext.Request.QueryString}";

                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("SourceIp", request.HttpContext.Connection.RemoteIpAddress));
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("UserAgent", request.HttpContext.Request.Headers["User-Agent"]));
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("RawUrl", rawURL));
            }
        }
    }
}