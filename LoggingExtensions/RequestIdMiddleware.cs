using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;

namespace LoggingExtensions
{
    public static class RequestIdMiddleware
    {
        /// <summary>
        /// Add a middleware that adds the trace identifier from the HTTP context to the response headers.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="optionsAction"></param>
        public static void UseRequestIdMiddleware(this IApplicationBuilder app, Action<RequestIdOptions>? optionsAction = null)
        {
            var options = new RequestIdOptions();

            optionsAction?.Invoke(options);

            app.Use(async (context, next) =>
            {
                var traceIdentifier = Activity.Current?.Id ?? context.TraceIdentifier;
                context.Response.Headers.Add(options.HeaderKey, traceIdentifier);
                await next();
            });
        }
    }
}