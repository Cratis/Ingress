// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;

namespace Cratis.Ingress.Identity.for_IdentityDetailsResolver;

public class when_identity_cookie_is_already_present : Specification
{
    IdentityDetailsResolver _resolver;
    DefaultHttpContext _context;
    IHttpClientFactory _httpClientFactory;
    bool _result;

    void Establish()
    {
        var config = new IngressConfig
        {
            Microservices = new Dictionary<string, MicroserviceConfig>
            {
                ["main"] = new() { Backend = new MicroserviceEndpointConfig { BaseUrl = "http://backend" } }
            }
        };
        var optionsMonitor = Substitute.For<IOptionsMonitor<IngressConfig>>();
        optionsMonitor.CurrentValue.Returns(config);

        _httpClientFactory = Substitute.For<IHttpClientFactory>();

        _resolver = new IdentityDetailsResolver(optionsMonitor, _httpClientFactory, Substitute.For<ILogger<IdentityDetailsResolver>>());

        _context = new DefaultHttpContext();
        _context.Request.Headers["Cookie"] = $"{Cookies.Identity}=existing-value";
    }

    async Task Because() => _result = await _resolver.Resolve(_context, new ClientPrincipal { UserId = "user-1" }, Guid.NewGuid());

    [Fact] void should_return_true() => Assert.True(_result);
    [Fact] void should_not_call_the_http_client() => _httpClientFactory.DidNotReceive().CreateClient(Arg.Any<string>());
}
