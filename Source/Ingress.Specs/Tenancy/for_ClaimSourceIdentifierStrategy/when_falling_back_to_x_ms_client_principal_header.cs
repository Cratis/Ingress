// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Ingress.Tenancy.for_ClaimSourceIdentifierStrategy;

public class when_falling_back_to_x_ms_client_principal_header : Specification
{
    ClaimSourceIdentifierStrategy _strategy;
    DefaultHttpContext _context;
    bool _succeeded;
    string _sourceIdentifier;

    void Establish()
    {
        _strategy = new ClaimSourceIdentifierStrategy();
        _context = new DefaultHttpContext();

        var principal = new ClientPrincipal
        {
            IdentityProvider = "aad",
            UserId = "user-123",
            UserDetails = "user@example.com",
            Claims = [new ClientPrincipalClaim { Type = "http://schemas.microsoft.com/identity/claims/tenantid", Value = "header-tenant" }]
        };
        _context.Request.Headers[Headers.Principal] = principal.ToBase64();
    }

    void Because() => _succeeded = _strategy.TryResolveSourceIdentifier(_context, [], out _sourceIdentifier);

    [Fact] void should_succeed() => Assert.True(_succeeded);
    [Fact] void should_return_the_claim_value_from_header() => Assert.Equal("header-tenant", _sourceIdentifier);
}
