using Asp.Versioning;
using Bookify.Api.Extensions;
using Bookify.Application;
using Bookify.Aspire.ServiceDefaults;
using Bookify.Infrastructure;
using Carter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Host.UseSerilog(
    ((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration))
);

builder.Services.AddOpenApi("v1");
builder.Services.AddOpenApi("v2");

builder.Services.AddApplication();
builder.AddInfrastructure();

builder.Services.AddCors(options =>
    options.AddPolicy(
        "CorsPolicy",
        corsPolicyBuilder =>
        {
            corsPolicyBuilder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
        }
    )
);

builder.Services.AddCarter();

var app = builder.Build();

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

// app.UseRequestContextLogging();

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

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });

    app.UseReDoc(options =>
    {
        options.SpecUrl("/openapi/v1.json");
    });

    app.MapScalarApiReference(options =>
    {
        options.WithTheme(ScalarTheme.Mars);

        options.Servers = [new ScalarServer("https://localhost:7039")];
    });
    app.ApplyMigrations();
    app.SeedData();
}

app.Run();

public partial class Program;
