// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Ingress.Identity;
using Yarp.ReverseProxy.Configuration;

namespace Cratis.Ingress.ReverseProxy;

/// <summary>
/// Extension methods for wiring up the YARP reverse proxy.
/// </summary>
public static class ReverseProxyExtensions
{
    /// <summary>
    /// Registers the YARP reverse proxy, the custom config provider and
    /// the <see cref="InjectIdentityHeadersTransform"/> transform.
    /// </summary>
    public static WebApplicationBuilder SetupReverseProxy(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<MicroserviceReverseProxyConfigProvider>();
        builder.Services.AddSingleton<IProxyConfigProvider>(
            sp => sp.GetRequiredService<MicroserviceReverseProxyConfigProvider>());

        builder.Services
            .AddReverseProxy()
            .ConfigureHttpClient((_, handler) =>
            {
                handler.KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always;
                handler.KeepAlivePingDelay = TimeSpan.FromMinutes(1);
                handler.KeepAlivePingTimeout = TimeSpan.FromMinutes(2);
                handler.ConnectTimeout = TimeSpan.FromMinutes(3);
            })
            .AddTransforms(ctx =>
            {
                ctx.RequestTransforms.Add(new InjectIdentityHeadersTransform());
            });

        return builder;
    }

    /// <summary>
    /// Maps the YARP reverse proxy middleware into the pipeline.
    /// </summary>
    public static WebApplication UseReverseProxy(this WebApplication app)
    {
        app.MapReverseProxy();
        return app;
    }
}
