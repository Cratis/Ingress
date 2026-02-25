// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Ingress.Invites;

/// <summary>
/// Defines a validator for invite JWT tokens issued by Cratis Studio.
/// </summary>
public interface IInviteTokenValidator
{
    /// <summary>
    /// Validates the invite <paramref name="token"/> against the configured public key and claims.
    /// </summary>
    /// <param name="token">The raw JWT string to validate.</param>
    /// <returns><c>true</c> if the token is valid; otherwise <c>false</c>.</returns>
    bool Validate(string token);
}
