// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Ingress.Configuration;
using Cratis.Ingress.Identity;
using Microsoft.Extensions.Options;

namespace Cratis.Ingress;

/// <summary>
/// Middleware that runs after authentication on every request to:
/// <list type="number">
///   <item>Strip inbound identity headers so clients cannot spoof them.</item>
///   <item>Resolve the tenant from the request and store it in <see cref="HttpContext.Items"/>.</item>
///   <item>Call <c>/.cratis/me</c> on configured microservices and write the identity cookie.</item>
/// </list>
/// </summary>
public class TenancyMiddleware(
    RequestDelegate next,
    IOptionsMonitor<IngressConfig> config,
    ITenantResolver tenantResolver,
    IIdentityDetailsResolver identityDetailsResolver,
    ILogger<TenancyMiddleware> logger)
{
    /// <summary>Key used to store the resolved tenant ID in <see cref="HttpContext.Items"/>.</summary>
    public const string TenantIdItemKey = "Cratis.TenantId";

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Always strip inbound spoofable headers before any downstream sees them.
        context.Request.Headers.Remove(Headers.Principal);
        context.Request.Headers.Remove(Headers.PrincipalId);
        context.Request.Headers.Remove(Headers.PrincipalName);
        context.Request.Headers.Remove(Headers.TenantId);

        // 2. Resolve tenant.
        if (!tenantResolver.TryResolve(context, out var tenantId))
        {
            if (config.CurrentValue.TenantResolutions.Count > 0)
            {
                logger.LogWarning("Could not resolve tenant for request {Path}. Returning 401.", context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
        }

        context.Items[TenantIdItemKey] = tenantId;

        // 3. If the user is authenticated, resolve identity details (call /.cratis/me).
        var principal = context.BuildClientPrincipal();
        if (principal is not null && tenantId != Guid.Empty)
        {
            var allowed = await identityDetailsResolver.Resolve(context, principal, tenantId);
            if (!allowed)
            {
                // 403 already written by the resolver.
                return;
            }
        }

        await next(context);
    }
}
