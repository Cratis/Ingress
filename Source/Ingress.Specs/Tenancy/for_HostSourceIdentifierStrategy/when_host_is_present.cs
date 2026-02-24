// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Ingress.Tenancy.for_HostSourceIdentifierStrategy;

public class when_host_is_present : Specification
{
    HostSourceIdentifierStrategy _strategy;
    DefaultHttpContext _context;
    bool _succeeded;
    string _sourceIdentifier;

    void Establish()
    {
        _strategy = new HostSourceIdentifierStrategy();
        _context = new DefaultHttpContext();
        _context.Request.Host = new HostString("my-tenant.example.com");
    }

    void Because() => _succeeded = _strategy.TryResolveSourceIdentifier(_context, [], out _sourceIdentifier);

    [Fact] void should_succeed() => Assert.True(_succeeded);
    [Fact] void should_return_the_host_name() => Assert.Equal("my-tenant.example.com", _sourceIdentifier);
}
