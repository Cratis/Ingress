// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json.Nodes;
using Cratis.Ingress.Configuration;

namespace Cratis.Ingress.Tenancy;

/// <summary>
/// Always resolves to the tenant ID specified in the <c>tenantId</c> option.
/// Used for single-tenant deployments.
/// </summary>
public class SpecifiedSourceIdentifierStrategy : ISourceIdentifierStrategy
{
    /// <inheritdoc/>
    public TenantSourceIdentifierResolverType Type => TenantSourceIdentifierResolverType.Specified;

    /// <inheritdoc/>
    public bool TryResolveSourceIdentifier(HttpContext context, JsonObject options, out string sourceIdentifier)
    {
        sourceIdentifier = options["tenantId"]?.GetValue<string>() ?? string.Empty;
        return !string.IsNullOrEmpty(sourceIdentifier);
    }
}
