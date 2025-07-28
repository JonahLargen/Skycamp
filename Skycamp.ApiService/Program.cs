using FastEndpoints;
using FastEndpoints.Swagger;
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
        o.MaxEndpointVersion = 1;
        o.DocumentSettings = s =>
        {
            s.Title = "Skycamp API v1";
            s.Version = "v1";
            s.DocumentName = "v1";
        };
    })
    .SwaggerDocument(o =>
    {
        o.MaxEndpointVersion = 2;
        o.DocumentSettings = s =>
        {
            s.Title = "Skycamp API v2";
            s.Version = "v2";
            s.DocumentName = "v2";
        };
    });

// Shared Services
builder.Services.AddWeatherServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Configure FastEndpoints with versioning
app.UseFastEndpoints(c =>
{
    c.Versioning.Prefix = "v";
});

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.MapDefaultEndpoints();

app.Run();