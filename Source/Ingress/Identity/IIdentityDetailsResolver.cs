// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Ingress.Identity;

/// <summary>
/// Defines the contract for resolving additional identity details from a microservice's
/// <c>/.cratis/me</c> endpoint and persisting them as the <c>.cratis-identity</c> cookie.
/// </summary>
public interface IIdentityDetailsResolver
{
    /// <summary>
    /// Calls <c>/.cratis/me</c> on every configured microservice that exposes an
    /// identity details endpoint, merges the results and writes (or refreshes) the
    /// <c>.cratis-identity</c> response cookie.
    /// Returns <c>false</c> when the microservice reports that the user is forbidden (HTTP 403).
    /// </summary>
    Task<bool> Resolve(HttpContext context, ClientPrincipal principal, Guid tenantId);
}
