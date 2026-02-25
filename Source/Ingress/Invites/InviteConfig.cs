// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Ingress.Invites;

/// <summary>
/// Represents the configuration for the invite system.
/// </summary>
public class InviteConfig
{
    /// <summary>
    /// Gets or sets the RSA public key PEM used to verify invite token signatures.
    /// </summary>
    public string PublicKeyPem { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expected token issuer (<c>iss</c> claim).
    /// Leave empty to skip issuer validation.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expected token audience (<c>aud</c> claim).
    /// Leave empty to skip audience validation.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the absolute URL of the Studio invite-exchange endpoint,
    /// e.g. <c>https://studio.example.com/internal/invites/exchange</c>.
    /// </summary>
    public string ExchangeUrl { get; set; } = string.Empty;
}
