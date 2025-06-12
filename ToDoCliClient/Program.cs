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
        //todoClient.CreateList(new() { ListName = "List from CLI" });
        while (true)
        {
            Console.WriteLine("Hello In ToDoList-App" +
                "\n1->To create a new list" +
                "\n2->To Add new Item to a List" +
                "\n3-To ReadItem" +
                "\n4-To ReadListsWithItems" +
                "\n5-update an item" +
                "\n6-Delete an Item" +
                "\n7-Mark Item as done");

            int Choice = int.Parse(Console.ReadLine());
            IToDoServiceClient serves = new ToDoServiceClient(todoClient);

            switch (Choice)
            {
                case 1:
                    {
                        Console.WriteLine("You have to Enter List_name:");
                        string list_name = Console.ReadLine();
                        serves.CreateList(list_name);
                        break;
                    }
                case 2:
                    {
                        Console.WriteLine("Enter Id of List:");
                        Int32 list_id = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter ItemName:");
                        string item_name = Console.ReadLine();
                        serves.AddItem(list_id, item_name);
                        break;
                    }
                case 3:
                    {
                        Console.WriteLine("Enter Id of List:");
                        Int32 list_id = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter Id of Item:");
                        Int32 item_id = int.Parse(Console.ReadLine());
                        serves.ReadItem(list_id, item_id);
                        break;
                    }
                case 4:
                    {
                        serves.ReadLists();
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

                        serves.UpdateItem(item_id, list_id, item_name, item_is_done);
                        break;
                    }
                case 6:
                    {
                        serves.RemindMe();
                        Console.WriteLine("Enter Id of List:");
                        Int32 list_id = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter Id of item:");
                        Int32 item_id = int.Parse(Console.ReadLine());
                        serves.DeleteItem(list_id, item_id);
                        break;

                    }
                case 7:
                    {
                        serves.RemindMe();
                        Console.WriteLine("Enter Id of List:");
                        Int32 list_id = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter Id of item:");
                        Int32 item_id = int.Parse(Console.ReadLine());
                        serves.MarkAsDone(list_id, item_id);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
            Console.WriteLine("Do you want" +
                "\n1-To do another service" +
                "\n2-To Exit App");
            int.TryParse(Console.ReadLine(), out int app_result);
            if (app_result == 1)
            { continue; }
            else
            {
                break;
            }
        }

        Console.ReadLine();
    }
}
