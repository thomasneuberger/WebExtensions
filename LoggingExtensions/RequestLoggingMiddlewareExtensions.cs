using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace LoggingExtensions;

/// <summary>
/// Extensions for integrating the <see cref="RequestLoggingMiddleware"/> into the project.
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    /// <summary>
    /// Add the middleware to the service container.
    /// </summary>
    /// <param name="services">The service container.</param>
    /// <param name="options">The configuration for the middleware.</param>
    public static void AddRequestLoggingMiddleware(this IServiceCollection services, RequestLoggingOptions? options = null)
    {
        services.AddSingleton<RequestLoggingMiddleware>();
        services.AddSingleton(options ?? new RequestLoggingOptions());
    }
    
    /// <summary>
    /// Add the middleware to the request pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    public static void UseRequestLoggingMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestLoggingMiddleware>();
    }
}