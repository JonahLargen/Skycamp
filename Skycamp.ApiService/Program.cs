using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using FluentValidation;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Skycamp.ApiService.BackgroundServices;
using Skycamp.ApiService.Common.Logging;
using Skycamp.ApiService.Common.Tracing;
using Skycamp.ApiService.Common.Validation;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;
using Skycamp.ApiService.Hubs;
using Skycamp.ApiService.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

//Db Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("sqldb");

    options.UseSqlServer(connectionString);
});

//Hangfire
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("sqldb")));
builder.Services.AddHangfireServer();

//SignalR
builder.Services.AddSignalR();

// Configure Identity to use ApplicationUser and ApplicationDbContext
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+|";
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddAuthenticationJwtBearer(signingOptions =>
{
    //signingOptions.SigningKey = "W3L8pJrXZ97bqK1e2yM5vN4sQ6tFhG0zT7cRjP8kSaVwEoU9";
}, bearerOptions =>
{
    bearerOptions.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
    bearerOptions.Audience = builder.Configuration["Auth0:Audience"];
    bearerOptions.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name",
        RoleClaimType = "roles"
    };
});
builder.Services.AddAuthorization();

// Add FastEndpoints
builder.Services.AddFastEndpoints()
    .SwaggerDocument(o =>
    {
        o.EndpointFilter = ep =>
        {
            return ep.Version.Current == 1;
        };
        o.MinEndpointVersion = 1;
        o.MaxEndpointVersion = 1;
        o.DocumentSettings = s =>
        {
            s.Title = "Skycamp API v1";
            s.Version = "v1";
            s.DocumentName = "v1";
        };
        o.ShortSchemaNames = true;
    })
    .SwaggerDocument(o =>
    {
        o.EndpointFilter = ep =>
        {
            return ep.Version.Current == 2;
        };
        o.MinEndpointVersion = 2;
        o.MaxEndpointVersion = 2;
        o.DocumentSettings = s =>
        {
            s.Title = "Skycamp API v2";
            s.Version = "v2";
            s.DocumentName = "v2";
        };
        o.ShortSchemaNames = true;
    });

//Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Add Command Middleware
builder.Services.AddCommandMiddleware(c =>
{
    c.Register(typeof(CommandTracingMiddleware<,>));
    c.Register(typeof(CommandLoggingMiddleware<,>));
    c.Register(typeof(CommandValidationMiddleware<,>));
});

//Service Bus
builder.AddAzureServiceBusClient(connectionName: "sbemulatorns");

//Hosted Services
builder.Services.AddHostedService<OutboxSubscriber1>();
builder.Services.AddHostedService<OutboxSubscriber2>();
builder.Services.AddHostedService<FeedSubscriber>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

// Configure FastEndpoints with versioning & processors
app.UseFastEndpoints(c =>
{
    c.Versioning.Prefix = "v";
    c.Endpoints.Configurator = (ep) =>
    {
        ep.PreProcessor<GlobalTracingPreProcessor>(Order.Before);
        ep.PreProcessor<GlobalLoggingPreProcessor>(Order.Before);
        ep.PostProcessor<GlobalTracingPostProcessor>(Order.After);
        ep.PostProcessor<GlobalLoggingPostProcessor>(Order.After);
    };
});

app.UseHangfireDashboard();

var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();

recurringJobManager.AddOrUpdate<OutboxPublisherJob>(
    "OutboxPublisherJob",
    job => job.PublishUnprocessedMessagesAsync(),
    "*/5 * * * * *"
);

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.MapDefaultEndpoints();

//Map Hubs
app.MapHub<FeedHub>("/hubs/feed");

app.Run();