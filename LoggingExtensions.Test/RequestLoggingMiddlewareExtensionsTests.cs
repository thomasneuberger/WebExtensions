using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace LoggingExtensions.Test;

/// <summary>
/// Unit tests for <see cref="RequestLoggingMiddlewareExtensions"/>
/// </summary>
public class RequestLoggingMiddlewareExtensionsTests
{
    /// <summary>
    /// Test that <see cref="RequestLoggingMiddlewareExtensions.AddRequestLoggingMiddleware"/>
    /// adds the services to the DI container.
    /// </summary>
    [Fact]
    public void AddRequestLoggingMiddleware_AddsServices_WithDefaultOptions()
    {
        // Arrange
        var serviceCollectionMock = new Mock<IServiceCollection>();

        // Act
        serviceCollectionMock.Object.AddRequestLoggingMiddleware();

        //Assert
        serviceCollectionMock
            .Verify(s => s.Add(It.Is<ServiceDescriptor>(d => 
                d.ServiceType == typeof(RequestLoggingOptions)
                && d.ImplementationInstance != null
                && d.Lifetime == ServiceLifetime.Singleton)));
        serviceCollectionMock
            .Verify(s => s.Add(It.Is<ServiceDescriptor>(d => 
                d.ServiceType == typeof(RequestLoggingMiddleware)
                && d.ImplementationType == typeof(RequestLoggingMiddleware)
                && d.Lifetime == ServiceLifetime.Singleton)));
    }
    /// <summary>
    /// Test that <see cref="RequestLoggingMiddlewareExtensions.AddRequestLoggingMiddleware"/>
    /// adds the services to the DI container.
    /// </summary>
    [Fact]
    public void AddRequestLoggingMiddleware_AddsServices_WithCustomOptions()
    {
        // Arrange
        var serviceCollectionMock = new Mock<IServiceCollection>();
        var options = new RequestLoggingOptions();

        // Act
        serviceCollectionMock.Object.AddRequestLoggingMiddleware(options);

        //Assert
        serviceCollectionMock
            .Verify(s => s.Add(It.Is<ServiceDescriptor>(d => 
                d.ServiceType == typeof(RequestLoggingOptions)
                && d.ImplementationInstance == options
                && d.Lifetime == ServiceLifetime.Singleton)));
        serviceCollectionMock
            .Verify(s => s.Add(It.Is<ServiceDescriptor>(d => 
                d.ServiceType == typeof(RequestLoggingMiddleware)
                && d.ImplementationType == typeof(RequestLoggingMiddleware)
                && d.Lifetime == ServiceLifetime.Singleton)));
    }

    /// <summary>
    /// Test that <see cref="RequestLoggingMiddlewareExtensions.AddRequestLoggingMiddleware"/>
    /// adds the middleware to the pipeline.
    /// </summary>
    [Fact]
    public void UseRequestLoggingMiddleware_AddsMiddleware_ToPipeline()
    {
        // Arrange
        var appBuilderMock = new Mock<IApplicationBuilder>();

        // Act
        appBuilderMock.Object.UseRequestLoggingMiddleware();

        //Assert
        appBuilderMock
            .Verify(a => a.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once);
    }
}