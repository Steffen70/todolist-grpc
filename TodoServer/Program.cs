using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using SwissPension.Todo.Server.Data;
using SwissPension.Todo.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.ListenLocalhost(8443, listenOptions =>
{
    var pfxFile = Path.Combine(AppContext.BaseDirectory, "cert", "localhost.pfx");
    var logger = listenOptions.ApplicationServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"Using certificate from {pfxFile}");

    listenOptions.UseHttps(new X509Certificate2(pfxFile, ""));
    listenOptions.Protocols = HttpProtocols.Http2;
}));

builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<GreeterService>();
app.MapGrpcService<TodoService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
