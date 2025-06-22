using Bookify.Aspire.AppHost;
using Bookify.Aspire.AppHost.Extensions;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var dbUserName = builder.AddParameter("db-username", secret: false);
var dbPassword = builder.AddParameter("db-password", secret: true);

var postgres = builder
    .AddPostgres("postgres", dbUserName, dbPassword, 5433)
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("bookify");

var cache = builder
    .AddRedis("cache", 6379)
    .WithDataVolume(isReadOnly: true)
    .WithRedisInsight()
    .WithLifetime(ContainerLifetime.Persistent);

var keycloak = builder
    .AddKeycloak("keycloak", 18080)
    .WithRealmImport("./bookify-realm-export.json")
    .WithDataVolume()
    .WithExternalHttpEndpoints()
    .WithLifetime(ContainerLifetime.Persistent);

var seq = builder
    .AddSeq("seq", 5341)
    .ExcludeFromManifest()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEnvironment("ACCEPT_EULA", "Y");

var api = builder
    .AddProject<Projects.Bookify_Api>("BookifyApi")
    .WithReference(postgres)
    .WithReference(cache)
    .WithReference(keycloak)
    .WithReference(seq)
    .WaitFor(postgres)
    .WaitFor(cache)
    .WaitFor(keycloak)
    .WaitFor(seq)
    .WithSwaggerUI()
    .WithScalar()
    .WithReDoc()
    .WithSwaggerUILink()
    .WithScalarLink()
    .WithReDocLink();

builder.Build().Run();
