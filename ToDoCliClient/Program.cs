using System.Security.Cryptography.X509Certificates;
using Grpc.Core;
using Grpc.Net.Client;
using ToDoGrpc;
using ToDoCliClient.methods;
using Azure;
using Google.Protobuf.Collections;

namespace ToDoCliClient;

public static class Program
{
    internal static void Main()
    {
        var baseAddress = new Uri("https://localhost:8443");

        // Load root CA
        using var rootCa = new X509Certificate2(Path.Combine(AppContext.BaseDirectory, "cert", "root_ca.crt"));

        // Create handler that trusts your custom cert
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (_, cert, chain, _) =>
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

        // Optional initial call (لو مش محتاجه ممكن تشيلها)
        todoClient.CreateList(new() { ListName = "List from CLI" });

        Console.WriteLine("Hello In ToDoList-App" +
            "\n1->To create a new list" +
            "\n2->To Add new Item to a List" +
            "\n3-To ReadItem" +
            "\n4-To ReadListsWithItems" +
            "\n5-update an item");

        int Choice = int.Parse(Console.ReadLine());

        switch (Choice)
        {
            case 1:
                {
                    Console.WriteLine("You have to Enter List_name:");
                    string list_name = Console.ReadLine();
                    ToDoServiceClient.CreateList(list_name, todoClient);
                    break;
                }
            case 2:
                {
                    Console.WriteLine("Enter Id of List:");
                    Int32 list_id = int.Parse(Console.ReadLine());
                    Console.WriteLine("Enter ItemName:");
                    string item_name = Console.ReadLine();
                    ToDoServiceClient.AddItem(list_id, item_name, todoClient);
                    break;
                }
            case 3:
                {
                    Console.WriteLine("Enter Id of List:");
                    Int32 list_id = int.Parse(Console.ReadLine());
                    Console.WriteLine("Enter Id of Item:");
                    Int32 item_id = int.Parse(Console.ReadLine());
                    ToDoServiceClient.ReadItem(list_id, item_id, todoClient);
                    break;
                }
            case 4:
                {
                    ToDoServiceClient.ReadLists(todoClient);
                    break;
                }
            case 5:
                {
                    Console.WriteLine("Enter Id of List:");
                    Int32 list_id = int.Parse(Console.ReadLine());
                    Console.WriteLine("Enter Id of item:");
                    Int32 item_id = int.Parse(Console.ReadLine());
                    Console.WriteLine("Enter ItemName:");
                    string item_name = Console.ReadLine();
                    Console.WriteLine("Enter the Status of the item (Done = true , Still working = false)");
                    bool item_is_done = bool.TryParse(Console.ReadLine(), out bool result) ? result : false;

                    ToDoServiceClient.UpdateItems(item_id, list_id, item_name, item_is_done, todoClient);
                    break;
                }
            default:
                {
                    break;
                }
        }

        Console.ReadLine();
    }
}
