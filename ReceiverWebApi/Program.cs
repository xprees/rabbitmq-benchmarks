using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize =
        null; // unlimited - but System.Text.Json doesn't support deserializing larger messages than 100MB
});

var app = builder.Build();

app.MapPost("/echo", ([FromBody] object data) => data)
    .Accepts<object>("application/json")
    .Produces<object>(200, "application/json");


app.MapPost("/receive", ([FromBody] object data) => Results.NoContent())
    .Accepts<object>("application/json")
    .Produces(204);

app.Run();