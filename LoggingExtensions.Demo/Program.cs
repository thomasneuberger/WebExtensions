using LoggingExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRequestLoggingMiddleware();

var app = builder.Build();

app.UseRequestIdMiddleware();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRequestLoggingMiddleware();

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Ok())
    .WithName("Root");

app.MapGet("/hello", (string name) => Results.Json($"Hello {name}"))
    .WithName("SayHello");

app.Run();