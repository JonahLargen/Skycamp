using FastEndpoints;
using Skycamp.ApiService.Features.Weather.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add FastEndpoints
builder.Services.AddFastEndpoints();

// Shared Services
builder.Services.AddWeatherServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Configure FastEndpoints with versioning
app.UseFastEndpoints(c =>
{
    c.Versioning.Prefix = "v";
});

app.MapDefaultEndpoints();

app.Run();