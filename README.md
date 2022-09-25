# Web Extensions

The Web Extensions are a collection of small tools to be integrated in ASP.NET projects.

## Logging Extensions

The logging extensions are extension methods to extend the ASP.NET request pipeline with middlewares in the context of logging.

### RequestIdMiddleware
The RequestId middleware adds the current activity Iid to the HTTP response headers. This id identifies a single request and can be attached to log entries.

The Id a string that consists of 4 parts, separated by dashes. 

* Application Insights: the second part is used as operationId.
* The id can be included in the logs with the layout renderer <code>${aspnet-TraceIdentifier:ignoreActivityId=boolean}</code>.

To use the middleware, add this line to the pipeline configuration as early as possible:

    app.UseRequestIdMiddleware();