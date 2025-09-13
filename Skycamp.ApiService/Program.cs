using FastEndpoints;
using FastEndpoints.Swagger;
using FluentValidation;
using Skycamp.ApiService.Common.Logging;
using Skycamp.ApiService.Common.Tracing;
using Skycamp.ApiService.Common.Validation;
using Skycamp.ApiService.Features.Weather.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.MapDefaultEndpoints();

app.Run();