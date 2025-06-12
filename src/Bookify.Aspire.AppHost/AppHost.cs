var builder = DistributedApplication.CreateBuilder(args);

var dbuserName = builder.AddParameter("db-username", secret: false);
var dbPassword = builder.AddParameter("db-password", secret: true);

var postgres = builder
    .AddPostgres("postgres", dbuserName, dbPassword, 5433)
    .WithDataVolume(isReadOnly: false)
    .AddDatabase("bookify");

var cache = builder.AddRedis("cache", 6379).WithDataVolume(isReadOnly: true);

var keycloak = builder
    .AddKeycloak("keycloak", 18080)
    .WithRealmImport("./bookify-realm-export.json")
    .WithDataVolume()
    .WithExternalHttpEndpoints();

var seq = builder
    .AddSeq("seq", 5341)
    .ExcludeFromManifest()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEnvironment("ACCEPT_EULA", "Y");

builder
    .AddProject<Projects.Bookify_Api>("BookifyApi")
    .WithExternalHttpEndpoints()
    .WithReference(postgres)
    .WithReference(cache)
    .WithReference(keycloak)
    .WithReference(seq);
builder.Build().Run();
