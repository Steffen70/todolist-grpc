using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using SwissPension.Todo.Server.Data;
using SwissPension.Todo.Server.Services;

const string corsPolicyName = "ClientPolicy";

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.ListenLocalhost(8445, listenOptions =>
{
    var pfxFile = Path.Combine(AppContext.BaseDirectory, "cert", "localhost.pfx");
    var logger = listenOptions.ApplicationServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"Using certificate from {pfxFile}");

    listenOptions.UseHttps(new X509Certificate2(pfxFile, "1234"));
    // Enable HTTP/2 and HTTP/1.1 for gRPC-Web compatibility
    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
}));

builder.Services.AddDbContext<AppDbContext>();

// Allow all origins
builder.Services.AddCors(o => o.AddPolicy(corsPolicyName, policyBuilder =>
{
    policyBuilder
        // Allow all ports on localhost
        .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
        // Allow all methods and headers
        .AllowAnyMethod()
        .AllowAnyHeader()
        // Expose the gRPC-Web headers
        .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));

builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Enable the HTTPS redirection - only use HTTPS
app.UseHttpsRedirection();

// Enable CORS - allow all origins and add gRPC-Web headers
app.UseCors(corsPolicyName);

// Enable gRPC-Web for all services
app.UseGrpcWeb(new() { DefaultEnabled = true });

app.MapGrpcService<GreeterService>();
app.MapGrpcService<TodoService>();

app.Run();
