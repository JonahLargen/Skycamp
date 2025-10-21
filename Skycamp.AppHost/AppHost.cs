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
    .WithHttpHealthCheck("/health")
    .WithEnvironment("Auth0__Domain", builder.Configuration["Auth0:Domain"])
    .WithEnvironment("Auth0__ClientId", builder.Configuration["Auth0:ClientId"])
    .WithEnvironment("Auth0__ClientSecret", builder.Configuration["Auth0:ClientSecret"])
    .WithEnvironment("Auth0__Audience", builder.Configuration["Auth0:Audience"]);

builder.AddProject<Projects.Skycamp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithEnvironment("Auth0__Domain", builder.Configuration["Auth0:Domain"])
    .WithEnvironment("Auth0__ClientId", builder.Configuration["Auth0:ClientId"])
    .WithEnvironment("Auth0__ClientSecret", builder.Configuration["Auth0:ClientSecret"])
    .WithEnvironment("Auth0__Audience", builder.Configuration["Auth0:Audience"]);

builder.Build().Run();
