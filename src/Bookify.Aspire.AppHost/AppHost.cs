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

var prometheus = builder
    .AddContainer("prometheus", "prom/prometheus")
    .WithBindMount("../../prometheus", "/etc/prometheus", isReadOnly: true)
    .WithArgs("--config.file=/etc/prometheus/prometheus.yml")
    .WithHttpEndpoint(9090, name: "prometheus", targetPort: 9090)
    .WithLifetime(ContainerLifetime.Persistent);

var grafana = builder
    .AddContainer("grafana", "grafana/grafana")
    .WithBindMount("../../grafana/config", "/etc/grafana", isReadOnly: true)
    .WithBindMount("../../grafana/dashboards", "/var/lib/grafana/dashboards", isReadOnly: true)
    .WithEnvironment("PROMETHEUS_ENDPOINT", prometheus.GetEndpoint("prometheus"))
    .WithHttpEndpoint(3000, name: "grafana", targetPort: 3000)
    .WithLifetime(ContainerLifetime.Persistent)
    .WaitFor(prometheus);

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
    .WaitFor(prometheus)
    .WaitFor(grafana)
    .WithSwaggerUI()
    .WithScalar()
    .WithReDoc()
    .WithSwaggerUILink()
    .WithScalarLink()
    .WithReDocLink();

builder.Build().Run();
