// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Ingress.Configuration;

/// <summary>
/// Represents the type / brand of an OIDC provider, used by the login page to pick the correct logo.
/// </summary>
public enum OidcProviderType
{
    /// <summary>A generic / unknown provider.</summary>
    Custom,

    /// <summary>Microsoft identity platform (Azure AD / Entra ID).</summary>
    Microsoft,

    /// <summary>Google identity.</summary>
    Google,

    /// <summary>GitHub OAuth / OIDC.</summary>
    GitHub,

    /// <summary>Apple Sign-In.</summary>
    Apple
}
