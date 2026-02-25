// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Ingress;

/// <summary>
/// Represents well-known cookie names used by the ingress.
/// </summary>
public static class Cookies
{
    /// <summary>
    /// Cookie holding the enriched identity details from the application's identity provider endpoint.
    /// </summary>
    public const string Identity = ".cratis-identity";

    /// <summary>
    /// Short-lived HTTP-only cookie used to carry the invite token across the OIDC redirect.
    /// </summary>
    public const string InviteToken = ".cratis-invite";
}
