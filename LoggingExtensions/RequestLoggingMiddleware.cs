using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace LoggingExtensions;

/// <summary>
/// Middleware to log HTTP requests including query strings.
/// </summary>
public sealed class RequestLoggingMiddleware : IMiddleware
{
    /// <summary>
    /// The logger to log to.
    /// </summary>
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    /// <summary>
    /// The configuration for the middleware.
    /// </summary>
    private readonly RequestLoggingOptions _options;
    /// <summary>
    /// If false, no log is written by this extension.
    /// </summary>
    private readonly bool _logEnabled;

    /// <summary>
    /// Instantiates an new <see cref="RequestLoggingMiddleware"/> object.
    /// </summary>
    /// <param name="logger">the logger to log to.</param>
    /// <param name="options">The configuration for the middleware. If null, a default configuration will be used.</param>
    /// <exception cref="ArgumentNullException">Thrown if logger is null.</exception>
    public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger, RequestLoggingOptions? options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger), $"The {nameof(RequestLoggingMiddleware)} needs a logger to work.");
        _options = options ?? new RequestLoggingOptions();
        
#if DEBUG
        _logEnabled = true;
#else
        _logEnabled = !_options.DebugOnly;   
#endif
    }

    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        Stopwatch? sw = null;
        
        if (_logEnabled)
        {
            if (_options.LogDuration)
            {
                sw = new Stopwatch();
                sw.Start();
            }
        
            LogRequest(context.Request);
        }
        
        await next(context);

        // sw is only set if logging is enabled and the duration should be logged.
        if (sw is not null)
        {
            sw.Stop();
            LogDuration(sw.Elapsed);
        }
    }

    /// <summary>
    /// Log the incoming request.
    /// </summary>
    /// <param name="request">The request.</param>
    private void LogRequest(HttpRequest request)
    {
        _logger.Log(
            _options.LogLevel,
            "{Method} request received for URL {Url}",
            request.Method,
            request.GetDisplayUrl());
    }

    /// <summary>
    /// Logs the duration.
    /// </summary>
    /// <param name="duration">The duration needed for processing the request.</param>
    private void LogDuration(TimeSpan duration)
    {
        _logger.Log(
            _options.LogLevel,
            "Response returned after {ProcessingTime:N0} ms", duration.TotalMilliseconds);
    }
}