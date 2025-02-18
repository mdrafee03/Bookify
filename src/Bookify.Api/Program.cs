using Bookify.Api.Extensions;
using Bookify.Application;
using Bookify.Infrastructure;
using Carter;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(
    ((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration))
);

builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCarter();

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

app.Run();
