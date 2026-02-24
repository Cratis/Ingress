// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Ingress;
using Cratis.Ingress.Configuration;
using Cratis.Ingress.Identity;
using Cratis.Ingress.ReverseProxy;
using Cratis.Ingress.Tenancy;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;

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
    });

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
app.UseAuthentication();
app.UseAuthorization();

// TenancyMiddleware strips spoofable inbound headers, resolves the tenant and
// calls /.cratis/me to enrich the identity cookie – runs after auth so the
// ClaimsPrincipal is available.
app.UseMiddleware<TenancyMiddleware>();

app.UseReverseProxy();

await app.RunAsync();
