// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Ingress.Configuration;

/// <summary>
/// Represents the root configuration for the ingress.
/// </summary>
public class IngressConfig
{
    /// <summary>
    /// Gets or sets the <see cref="TenantsConfig"/>.
    /// Tenants are keyed by tenant ID (GUID).
    /// </summary>
    public TenantsConfig Tenants { get; set; } = new();

    /// <summary>
    /// Gets or sets the tenant resolution strategies applied in order until one resolves.
    /// </summary>
    public IList<TenantResolutionConfig> TenantResolutions { get; set; } = [];

    /// <summary>
    /// Gets or sets the microservices configuration.
    /// Microservices are keyed by a friendly name (e.g. "portal", "catalog").
    /// </summary>
    public IDictionary<string, MicroserviceConfig> Microservices { get; set; } = new Dictionary<string, MicroserviceConfig>();

    /// <summary>
    /// Gets or sets the list of OIDC providers available for login.
    /// When more than one provider is configured the ingress redirects unauthenticated
    /// browser requests to the login selection page instead of challenging directly.
    /// </summary>
    public IList<OidcProviderConfig> OidcProviders { get; set; } = [];
}
