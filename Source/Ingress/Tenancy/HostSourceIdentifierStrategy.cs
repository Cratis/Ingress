// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json.Nodes;
using Cratis.Ingress.Configuration;

namespace Cratis.Ingress.Tenancy;

/// <summary>
/// Resolves the tenant source identifier from the HTTP request host name.
/// </summary>
public class HostSourceIdentifierStrategy : ISourceIdentifierStrategy
{
    /// <inheritdoc/>
    public TenantSourceIdentifierResolverType Type => TenantSourceIdentifierResolverType.Host;

    /// <inheritdoc/>
    public bool TryResolveSourceIdentifier(HttpContext context, JsonObject options, out string sourceIdentifier)
    {
        sourceIdentifier = context.Request.Host.Host;
        return !string.IsNullOrEmpty(sourceIdentifier);
    }
}
