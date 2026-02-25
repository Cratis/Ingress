// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Ingress;
using Cratis.Ingress.Configuration;
using Cratis.Ingress.Identity;
using Cratis.Ingress.ReverseProxy;
using Cratis.Ingress.Tenancy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// ── Configuration ──────────────────────────────────────────────────────────
builder.Services
    .AddOptions<IngressConfig>()
    .BindConfiguration("Ingress")
    .ValidateDataAnnotations()
    .ValidateOnStart();

// ── Authentication ─────────────────────────────────────────────────────────
// The ingress supports two authentication flows:
//   • Interactive browser sessions via OIDC + cookie (default for frontends).
//   • Machine-to-machine / API via JWT Bearer.
// Both can be active simultaneously; the middleware challenges with OIDC for
// browser requests and with Bearer for API calls.
var authBuilder = builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

        // When multiple OIDC providers are configured, redirect to the login
        // selection page instead of directly challenging with a single provider.
        options.Events.OnRedirectToLogin = ctx =>
        {
            var ingressConfig = ctx.HttpContext.RequestServices
                .GetRequiredService<IOptionsMonitor<IngressConfig>>()
                .CurrentValue;

            if (ingressConfig.OidcProviders.Count > 1)
            {
                var returnUrl = ctx.Request.Path + ctx.Request.QueryString;
                ctx.Response.Redirect($"{WellKnownPaths.LoginPage}?returnUrl={Uri.EscapeDataString(returnUrl)}");
                return Task.CompletedTask;
            }

            ctx.Response.Redirect(ctx.RedirectUri);
            return Task.CompletedTask;
        };
    });

// ── Single-provider OIDC (legacy / single-provider configuration) ──────────
var oidcSection = builder.Configuration.GetSection("Authentication:OpenIdConnect");
if (oidcSection.Exists())
{
    authBuilder.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        oidcSection.Bind(options);
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
    });
}

// ── Multi-provider OIDC (Ingress:OidcProviders) ────────────────────────────
// Each provider is registered as its own OIDC scheme so that the ingress can
// challenge with the correct one based on the provider the user selected.
var multiProviders = builder.Configuration
    .GetSection("Ingress:OidcProviders")
    .Get<List<OidcProviderConfig>>() ?? [];

foreach (var provider in multiProviders)
{
    var schemeName = OidcProviderScheme.FromName(provider.Name);
    authBuilder.AddOpenIdConnect(schemeName, options =>
    {
        options.Authority = provider.Authority;
        options.ClientId = provider.ClientId;
        options.ClientSecret = provider.ClientSecret;
        options.ResponseType = "code";
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        foreach (var scope in provider.Scopes)
        {
            options.Scope.Add(scope);
        }

        options.CallbackPath = $"/signin-{schemeName}";
    });
}

var jwtSection = builder.Configuration.GetSection("Authentication:JwtBearer");
if (jwtSection.Exists())
{
    authBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        jwtSection.Bind(options);
    });
}

builder.Services.AddAuthorization(options =>
{
    // "default" policy requires an authenticated user.
    options.AddPolicy("default", policy => policy.RequireAuthenticatedUser());
});

// ── Tenancy ────────────────────────────────────────────────────────────────
builder.Services.AddSingleton<ISourceIdentifierStrategy, HostSourceIdentifierStrategy>();
builder.Services.AddSingleton<ISourceIdentifierStrategy, ClaimSourceIdentifierStrategy>();
builder.Services.AddSingleton<ISourceIdentifierStrategy, RouteSourceIdentifierStrategy>();
builder.Services.AddSingleton<ISourceIdentifierStrategy, SpecifiedSourceIdentifierStrategy>();
builder.Services.AddSingleton<ITenantResolver, TenantResolver>();

// ── Identity ───────────────────────────────────────────────────────────────
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IIdentityDetailsResolver, IdentityDetailsResolver>();

// ── Reverse proxy ──────────────────────────────────────────────────────────
builder.SetupReverseProxy();

// ── Forwarded headers (when running behind a load-balancer / nginx) ────────
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

// ── Application ────────────────────────────────────────────────────────────
var app = builder.Build();

app.UseForwardedHeaders();

// Serve the bundled Web SPA assets (wwwroot) so the login selection page is available.
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

// TenancyMiddleware strips spoofable inbound headers, resolves the tenant and
// calls /.cratis/me to enrich the identity cookie – runs after auth so the
// ClaimsPrincipal is available.
app.UseMiddleware<TenancyMiddleware>();

// ── OIDC providers endpoint ────────────────────────────────────────────────
// Returns a JSON array of OidcProviderInfo objects that the login page uses to
// render the list of available login options.
app.MapGet(WellKnownPaths.Providers, (IOptionsMonitor<IngressConfig> config) =>
{
    var providers = config.CurrentValue.OidcProviders;
    var result = providers.Select(OidcProviderScheme.ToProviderInfo);

    return Results.Json(result);
});

// ── Per-provider login endpoints ───────────────────────────────────────────
// Initiates the OIDC challenge for the requested provider scheme.
app.MapGet($"{WellKnownPaths.LoginPrefix}/{{scheme}}", async (string scheme, HttpContext context) =>
{
    var returnUrl = context.Request.Query["returnUrl"].FirstOrDefault() ?? "/";
    var properties = new AuthenticationProperties { RedirectUri = returnUrl };
    await context.ChallengeAsync(scheme, properties);
});

// ── Login selection page (SPA fallback) ───────────────────────────────────
// Serves the bundled React app for any request under /.cratis/select-provider.
app.MapGet($"{WellKnownPaths.LoginPage}{{**path}}", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync(
        Path.Combine(app.Environment.WebRootPath, "index.html"));
});

app.UseReverseProxy();

await app.RunAsync();
