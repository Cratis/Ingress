// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Ingress.Tenancy.for_RouteSourceIdentifierStrategy;

public class when_path_matches_pattern : Specification
{
    RouteSourceIdentifierStrategy _strategy;
    DefaultHttpContext _context;
    JsonObject _options;
    bool _succeeded;
    string _sourceIdentifier;

    void Establish()
    {
        _strategy = new RouteSourceIdentifierStrategy();
        _context = new DefaultHttpContext();
        _context.Request.Path = "/acme-corp/api/orders";
        _options = new JsonObject { ["regularExpression"] = @"\/(?<sourceIdentifier>[\w-]+)\/" };
    }

    void Because() => _succeeded = _strategy.TryResolveSourceIdentifier(_context, _options, out _sourceIdentifier);

    [Fact] void should_succeed() => Assert.True(_succeeded);
    [Fact] void should_return_the_matched_segment() => Assert.Equal("acme-corp", _sourceIdentifier);
}
