// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Ingress.Tenancy.for_HostSourceIdentifierStrategy;

public class when_host_is_empty : Specification
{
    HostSourceIdentifierStrategy _strategy;
    DefaultHttpContext _context;
    bool _succeeded;
    string _sourceIdentifier;

    void Establish()
    {
        _strategy = new HostSourceIdentifierStrategy();
        _context = new DefaultHttpContext();
        _context.Request.Host = new HostString(string.Empty);
    }

    void Because() => _succeeded = _strategy.TryResolveSourceIdentifier(_context, [], out _sourceIdentifier);

    [Fact] void should_not_succeed() => Assert.False(_succeeded);
}
