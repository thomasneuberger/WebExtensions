using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace LoggingExtensions.Test;

public class RequestIdMiddlewareTests
{
    [Fact]
    public void UseRequestIdMiddleware_NoParameters_AddsDefaultHeader()
    {
        // Arrange
        var appMock = new Mock<IApplicationBuilder>();
        const string traceIdentifier = "My Identifier";

        Func<RequestDelegate, RequestDelegate>? middlewareDelegate = null;
        appMock
            .Setup(a => a.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
            .Callback((Func<RequestDelegate, RequestDelegate> m) => middlewareDelegate = m);
        
        // Act
        appMock.Object.UseRequestIdMiddleware();
        var response = CallMiddleware(middlewareDelegate, traceIdentifier);


        //Assert
        response.Headers.Should().Contain(header => header.Key == "X-RequestId" && header.Value == traceIdentifier);
    }

    [Fact]
    public void UseRequestIdMiddleware_ConfiguredKey_AddsConfiguredHeader()
    {
        // Arrange
        var appMock = new Mock<IApplicationBuilder>();
        const string traceIdentifier = "My Identifier";
        const string headerKey = "My Header";

        Func<RequestDelegate, RequestDelegate>? middlewareDelegate = null;
        appMock
            .Setup(a => a.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
            .Callback((Func<RequestDelegate, RequestDelegate> m) => middlewareDelegate = m);

        var configure = (RequestIdOptions options) =>
        {
            options.HeaderKey = headerKey;
        };
        
        // Act
        appMock.Object.UseRequestIdMiddleware(configure);
        var response = CallMiddleware(middlewareDelegate, traceIdentifier);


        //Assert
        response.Headers.Should().Contain(header => header.Key == headerKey && header.Value == traceIdentifier);
    }

    /// <summary>
    /// Call the middleware passed to the application builder
    /// </summary>
    /// <param name="middlewareDelegate">The middleware converted by the <see cref="UseExtensions"/></param>
    /// <param name="traceIdentifier">the trace identifier of the HTTP context</param>
    /// <returns>The response</returns>
    private static HttpResponse CallMiddleware(Func<RequestDelegate, RequestDelegate>? middlewareDelegate, string traceIdentifier)
    {
        var httpContext = new DefaultHttpContext
        {
            TraceIdentifier = traceIdentifier
        };
        var middleware = middlewareDelegate!(_ => Task.CompletedTask);
        middleware(httpContext);
        var response = httpContext.Response;
        return response;
    }
}