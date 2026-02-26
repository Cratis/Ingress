// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", (HttpContext ctx) =>
{
    var headers = ctx.Request.Headers
        .OrderBy(h => h.Key)
        .Select(h => new { name = h.Key, value = h.Value.ToString() });

    return Results.Json(new
    {
        path = ctx.Request.Path.Value,
        method = ctx.Request.Method,
        headers
    });
});

await app.RunAsync();
