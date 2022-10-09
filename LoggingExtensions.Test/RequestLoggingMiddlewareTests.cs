using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace LoggingExtensions.Test;

/// <summary>
/// Unit tests for <see cref="RequestLoggingMiddleware"/>
/// </summary>
public class RequestLoggingMiddlewareTests
{
    /// <summary>
    /// Tests if the constructor throws an <see cref="ArgumentNullException"/> if no logger is passed.
    /// </summary>
    [Fact]
    public void Constructor_Checks_Parameters()
    {
        // Arrange
        

        // Act
        var act = () => new RequestLoggingMiddleware(null!, null);

        //Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("logger");
    }

    /// <summary>
    /// Tests if <see cref="RequestLoggingMiddleware.InvokeAsync"/> logs the call with default settings.
    /// </summary>
    [Fact]
    public async Task InvokeAsync_DefaultOptions_LogsInformation()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RequestLoggingMiddleware>>();
        var options = new RequestLoggingOptions
        {
            // set to false to keep unit test the same with and without DEBUG
            DebugOnly = false
        };
        var sut = new RequestLoggingMiddleware(loggerMock.Object, options);
        HttpContext context = new DefaultHttpContext
        {
            Request =
            {
                Method = "GET",
                IsHttps = false,
                Host = new HostString("host"),
                Path = "/path",
                Query = new QueryCollection(new Dictionary<string, StringValues>
                {
                    {"q", "test"}
                })
            }
        };
        var nextCalled = false;

        async Task Next(HttpContext _)
        {
            nextCalled = true;
            await Task.CompletedTask;
        }

        // Act
        await sut.InvokeAsync(context, Next);

        //Assert
        nextCalled.Should().BeTrue();
        loggerMock
            .Verify(l => l.Log(
                options.LogLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString() == "GET request received for URL http://host/path?q=test"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        loggerMock
            .Verify(l => l.Log(
                options.LogLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString()!.StartsWith("Response returned after")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        loggerMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// Tests if <see cref="RequestLoggingMiddleware.InvokeAsync"/> logs the call with configured options.
    /// </summary>
    [Fact]
    public async Task InvokeAsync_ConfiguredOptions_LogsInformation()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RequestLoggingMiddleware>>();
        var options = new RequestLoggingOptions
        {
            LogLevel = LogLevel.Debug,
            LogDuration = false,
            // set to false to keep unit test the same with and without DEBUG
            DebugOnly = false
        };
        var sut = new RequestLoggingMiddleware(loggerMock.Object, options);
        HttpContext context = new DefaultHttpContext
        {
            Request =
            {
                Method = "GET",
                IsHttps = false,
                Host = new HostString("host"),
                Path = "/path",
                Query = new QueryCollection(new Dictionary<string, StringValues>
                {
                    {"q", "test"}
                })
            }
        };
        var nextCalled = false;

        async Task Next(HttpContext _)
        {
            nextCalled = true;
            await Task.CompletedTask;
        }

        // Act
        await sut.InvokeAsync(context, Next);

        //Assert
        nextCalled.Should().BeTrue();
        loggerMock
            .Verify(l => l.Log(
                options.LogLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString() == "GET request received for URL http://host/path?q=test"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        loggerMock.VerifyNoOtherCalls();
    }
}