using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Skycamp.Web;
using Skycamp.Web.Components;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

//Add Auth0 authentication
var auth0Domain = builder.Configuration["Auth0:Domain"];
var auth0ClientId = builder.Configuration["Auth0:ClientId"];

if (string.IsNullOrEmpty(auth0Domain) || string.IsNullOrEmpty(auth0ClientId))
{
    throw new InvalidOperationException("Auth0 configuration is missing. Please ensure Auth0:Domain and Auth0:ClientId are set in the configuration.");
}

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = auth0Domain;
    options.ClientId = auth0ClientId;
    options.OpenIdConnectEvents = new()
    {
       OnTokenValidated = async context =>
       {
           var apiClient = context.HttpContext.RequestServices.GetRequiredService<ApplicationApiClient>();
           var userId = context.Principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

           if (!string.IsNullOrEmpty(userId))
           {
               await apiClient.SyncUserAsync(userId);
           }

           await Task.CompletedTask;
       }
    };
});

builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/login";
    options.ReturnUrlParameter = "redirectUri";
});

builder.Services.AddCascadingAuthenticationState();

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRazorPages();

builder.Services.AddHttpClient<ApplicationApiClient>(client =>
    {
        // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
        // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
        client.BaseAddress = new("https+http://apiservice");
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapRazorPages();

app.MapDefaultEndpoints();

app.Run();
