using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace LoggingExtensions;

/// <summary>
/// Options to configure the request logging middleware. 
/// </summary>
public class RequestLoggingOptions
{
    /// <summary>
    /// The log level that should be used. Default: Information
    /// </summary>
    /// <seealso cref="LogLevel"/>
    public LogLevel LogLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// If true, the duration of request processing is logged. Default: true
    /// </summary>
    public bool LogDuration { get; set; } = true;
    
    /// <summary>
    /// If true, logging is only enabled in debugging. Default: true 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public bool DebugOnly { get; set; } = true;
}