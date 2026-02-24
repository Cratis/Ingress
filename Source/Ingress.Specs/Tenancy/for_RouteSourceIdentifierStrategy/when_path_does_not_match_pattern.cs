// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Ingress.Tenancy.for_RouteSourceIdentifierStrategy;

public class when_path_does_not_match_pattern : Specification
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
        _context.Request.Path = "/no-match-here";
        _options = new JsonObject { ["regularExpression"] = @"^\/tenants\/(?<sourceIdentifier>[\w-]+)\/" };
    }

    void Because() => _succeeded = _strategy.TryResolveSourceIdentifier(_context, _options, out _sourceIdentifier);

    [Fact] void should_not_succeed() => Assert.False(_succeeded);
    [Fact] void should_return_empty_source_identifier() => Assert.Empty(_sourceIdentifier);
}
