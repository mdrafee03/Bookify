using Bookify.Api;
using Bookify.Api.Extensions;
using Bookify.Api.HealthCheck;
using Bookify.Application;
using Bookify.Infrastructure;
using Carter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(
    ((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration))
);

builder.Services.AddOpenApi(
    "v1",
    options =>
    {
        options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    }
);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCarter();

builder
    .Services.AddHealthChecks()
    .AddCheck<SqlHealthCheck>("sql-health")
    .AddCheck<RedisCacheHealthCheck>("redis-health");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTheme(ScalarTheme.Mars);
        options.Servers =
        [
            new ScalarServer("https://localhost:7039"),
            new ScalarServer("http://localhost:5150"),
        ];
    });
    app.ApplyMigrations();
    // app.SeedData();
}

app.UseHttpsRedirection();

// app.UseSerilogRequestLogging();
app.UseRequestContextLogging();

app.UseCustomExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapCarter();

app.MapHealthChecks(
    "/health",
    new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse }
);

app.Run();
