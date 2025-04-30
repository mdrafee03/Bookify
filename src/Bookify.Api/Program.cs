using Asp.Versioning;
using Bookify.Api;
using Bookify.Api.Extensions;
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

builder.Services.AddOpenApi("v1");
builder.Services.AddOpenApi("v2");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCarter();

var app = builder.Build();

app.UseHttpsRedirection();

// app.UseSerilogRequestLogging();
app.UseRequestContextLogging();

app.UseCustomExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .HasApiVersion(new ApiVersion(2))
    .ReportApiVersions()
    .Build();

app.MapGroup("/v{version:apiVersion}").WithApiVersionSet(versionSet).MapCarter();

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

app.MapHealthChecks(
    "/health",
    new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse }
);

app.Run();
