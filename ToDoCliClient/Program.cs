using System.Security.Cryptography.X509Certificates;
using Grpc.Net.Client;
using ToDoGrpc;

var baseAddress = new Uri("https://localhost:8443");

// Load root CA
using var rootCa = new X509Certificate2(Path.Combine(AppContext.BaseDirectory, "cert", "root_ca.crt"));

// Create handler that trusts your custom cert
var handler = new HttpClientHandler();
handler.ServerCertificateCustomValidationCallback = (_, cert, chain, __) =>
{
    // Manually trust your own root CA
    chain!.ChainPolicy.ExtraStore.Add(rootCa);
    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
    chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

    var isValid = chain.Build(cert!);
    return isValid;
};

// Create gRPC channel using custom handler
var httpClient = new HttpClient(handler);
var channel = GrpcChannel.ForAddress(baseAddress, new()
{
    HttpClient = httpClient
});

// Use your client
var todoClient = new todoit.todoitClient(channel);

todoClient.CreateList(new() { ListName = "List from CLI" });

var result = todoClient.ReadLists(new());

foreach (var todoList in result.Lists)
{
    Console.WriteLine($"Found list: {todoList.ListName}");
}

Console.ReadLine();