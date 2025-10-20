using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.FluentUI.AspNetCore.Components;
using Skycamp.Web;
using Skycamp.Web.Components;
using Skycamp.Web.Services;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

//Add Auth0 authentication
var auth0Domain = builder.Configuration["Auth0:Domain"];
var auth0ClientId = builder.Configuration["Auth0:ClientId"];
var auth0ClientSecret = builder.Configuration["Auth0:ClientSecret"];
var auth0Audience = builder.Configuration["Auth0:Audience"];

if (string.IsNullOrEmpty(auth0Domain) || string.IsNullOrEmpty(auth0ClientId) || string.IsNullOrEmpty(auth0ClientSecret) || string.IsNullOrEmpty(auth0Audience))
{
    throw new InvalidOperationException("Auth0 configuration is missing. Please ensure Auth0:Domain, Auth0:ClientId, Auth0:ClientSecret, and Auth0:Audience are set in the configuration.");
}

builder.Services.AddScoped<WorkspaceStateService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<AccessTokenHandler>();

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = auth0Domain;
    options.ClientId = auth0ClientId;
    options.ClientSecret = auth0ClientSecret;
    options.Scope = "openid profile email offline_access";
    options.OpenIdConnectEvents = new()
    {
        OnTokenValidated = async context =>
        {
            var apiClient = context.HttpContext.RequestServices.GetRequiredService<ApplicationApiClient>();
            var sub = context.Principal?.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = context.Principal?.Claims.FirstOrDefault(c => c.Type == "email" || c.Type == ClaimTypes.Email)?.Value;
            var emailVerified = context.Principal?.Claims.FirstOrDefault(c => c.Type == "email_verified")?.Value == "true";
            var displayName = context.Principal?.Claims.FirstOrDefault(c => c.Type == "name" || c.Type == ClaimTypes.Name)?.Value;
            var avatarUrl = context.Principal?.Claims.FirstOrDefault(c => c.Type == "picture")?.Value;

            if (string.IsNullOrWhiteSpace(sub))
            {
                context.Fail("Sub claim is missing.");
                return;
            }

            var response = await apiClient.SyncUserAsync(new SyncUserRequest
            {
                LoginProvider = "auth0",
                ProviderKey = sub,
                Email = email,
                EmailVerified = emailVerified,
                DisplayName = displayName,
                AvatarUrl = avatarUrl
            });

            if (!response.IsSuccess)
            {
                context.Fail($"Failed to sync user: {response.ErrorMessage}");
                return;
            }

            var identity = (ClaimsIdentity)context.Principal!.Identity!;

            if (!identity.HasClaim(c => c.Type == "app_user_id"))
            {
                identity.AddClaim(new Claim("app_user_id", response.Data.UserId));
            }

            foreach (var role in response.Data.Roles)
            {
                if (!identity.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == role))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }

            await Task.CompletedTask;
        }
    };
})
.WithAccessToken(options =>
{
    options.Audience = auth0Audience;
    options.UseRefreshTokens = true;
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

builder.Services.AddHttpClient();
builder.Services.AddHttpClient<ApplicationApiClient>(client =>
    {
        // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
        // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
        client.BaseAddress = new("https+http://apiservice");
    })
    .AddHttpMessageHandler<AccessTokenHandler>();

builder.Services.AddFluentUIComponents();

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
