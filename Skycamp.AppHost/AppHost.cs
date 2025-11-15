using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);
var isDevelopment = builder.Environment.IsDevelopment();

var cache = builder.AddRedis("cache")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithRedisInsight(config =>
    {
        config.WithLifetime(ContainerLifetime.Persistent);
    });

#pragma warning disable ASPIREPROXYENDPOINTS001

var sql = builder.AddSqlServer("sqldb")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpointProxySupport(false) //https://github.com/dotnet/aspire/issues/7046
    .WithDataVolume();

#pragma warning restore ASPIREPROXYENDPOINTS001

var db = sql.AddDatabase("database");

var serviceBus = builder.AddAzureServiceBus(isDevelopment ? "sbemulatorns" : "messaging")
    .RunAsEmulator(config =>
    {
        config.WithConfiguration(node =>
        {
            var userConfig = node["UserConfig"]!;
            var ns = userConfig["Namespaces"]![0]!;
            var topics = ns["Topics"]!.AsArray();

            foreach (var topic in topics)
            {
                if ((string)topic!["Name"]! == "outbox")
                {
                    var topicProperties = topic["Properties"]!;

                    topicProperties["RequiresDuplicateDetection"] = true;
                    topicProperties["DuplicateDetectionHistoryTimeWindow"] = "PT5M";
                    topicProperties["DefaultMessageTimeToLive"] = "PT1H";
                }
            }
        });
        config.WithLifetime(ContainerLifetime.Persistent);
    });

var outboxTopic = serviceBus.AddServiceBusTopic("outbox");
var feedSubscription = outboxTopic.AddServiceBusSubscription("outbox-subscription-feed");
var activitySubscription = outboxTopic.AddServiceBusSubscription("outbox-subscription-activity");

var apiService = builder.AddProject<Projects.Skycamp_ApiService>("apiservice")
    .WithReference(db)
    .WaitFor(db)
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(serviceBus)
    .WaitFor(serviceBus)
    .WithHttpHealthCheck("/health")
    .WithEnvironment("Auth0__Domain", builder.Configuration["Auth0:Domain"])
    .WithEnvironment("Auth0__ClientId", builder.Configuration["Auth0:ClientId"])
    .WithEnvironment("Auth0__ClientSecret", builder.Configuration["Auth0:ClientSecret"])
    .WithEnvironment("Auth0__Audience", builder.Configuration["Auth0:Audience"]);

builder.AddProject<Projects.Skycamp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithEnvironment("Auth0__Domain", builder.Configuration["Auth0:Domain"])
    .WithEnvironment("Auth0__ClientId", builder.Configuration["Auth0:ClientId"])
    .WithEnvironment("Auth0__ClientSecret", builder.Configuration["Auth0:ClientSecret"])
    .WithEnvironment("Auth0__Audience", builder.Configuration["Auth0:Audience"])
    .WithEnvironment("Auth0__SuperUserEmail", builder.Configuration["Auth0:SuperUserEmail"])
    .WithEnvironment("Auth0__SuperUserPassword", builder.Configuration["Auth0:SuperUserPassword"]);

builder.Build().Run();
