// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Ingress;

/// <summary>
/// Represents well-known URL paths used by the ingress.
/// </summary>
public static class WellKnownPaths
{
    /// <summary>
    /// The well-known path for the Cratis identity details endpoint on a microservice.
    /// </summary>
    public const string IdentityDetails = "/.cratis/me";
}
