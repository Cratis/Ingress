// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Ingress.Tenancy.for_ClaimSourceIdentifierStrategy;

public class when_claim_is_not_found_anywhere : Specification
{
    ClaimSourceIdentifierStrategy _strategy;
    DefaultHttpContext _context;
    bool _succeeded;
    string _sourceIdentifier;

    void Establish()
    {
        _strategy = new ClaimSourceIdentifierStrategy();
        _context = new DefaultHttpContext();
    }

    void Because() => _succeeded = _strategy.TryResolveSourceIdentifier(_context, [], out _sourceIdentifier);

    [Fact] void should_not_succeed() => Assert.False(_succeeded);
    [Fact] void should_return_empty_source_identifier() => Assert.Empty(_sourceIdentifier);
}
