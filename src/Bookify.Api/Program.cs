using Bookify.Api.Extensions;
using Bookify.Application;
using Bookify.Infrastructure;
using Carter;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCarter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTheme(ScalarTheme.Mars);
    });
    app.ApplyMigrations();
    // app.SeedData();
}

app.UseHttpsRedirection();

app.UseCustomExceptionHandler();

app.MapCarter();

app.Run();
