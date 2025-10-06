var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

#pragma warning disable ASPIREPROXYENDPOINTS001
var sql = builder.AddSqlServer("sqldb")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpointProxySupport(false) //https://github.com/dotnet/aspire/issues/7046
    .WithDataVolume();
#pragma warning restore ASPIREPROXYENDPOINTS001

var db = sql.AddDatabase("database");

var apiService = builder.AddProject<Projects.Skycamp_ApiService>("apiservice")
    .WithReference(db)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Skycamp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
