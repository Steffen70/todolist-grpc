using System.Security.Cryptography.X509Certificates;
using Grpc.Net.Client;

namespace SwissPension.Todo.CliClient;

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
        var todoClient = new Common.Todo.TodoClient(channel);

        // Optional initial call (لو مش محتاجه ممكن تشيلها)
        //todoClient.CreateList(new() { ListName = "List from CLI" });
        while (true)
        {
            Console.WriteLine("Hello In TodoList-App" +
                              "\n1->To create a new list" +
                              "\n2->To Add new Item to a List" +
                              "\n3-To ReadItem" +
                              "\n4-To ReadListsWithItems" +
                              "\n5-update an item" +
                              "\n6-Delete an Item" +
                              "\n7-Mark Item as done");

            var choice = int.Parse(Console.ReadLine()!);
            ITodoServiceClient serves = new TodoServiceClient(todoClient);

            switch (choice)
            {
                case 1:
                {
                    Console.WriteLine("You have to Enter List_name:");
                    var listName = Console.ReadLine();
                    serves.CreateList(listName);
                    break;
                }
                case 2:
                {
                    Console.WriteLine("Enter Id of List:");
                    var listId = int.Parse(Console.ReadLine()!);
                    Console.WriteLine("Enter ItemName:");
                    var itemName = Console.ReadLine();
                    serves.AddItem(listId, itemName);
                    break;
                }
                case 3:
                {
                    Console.WriteLine("Enter Id of List:");
                    var listId = int.Parse(Console.ReadLine()!);
                    Console.WriteLine("Enter Id of Item:");
                    var itemId = int.Parse(Console.ReadLine()!);
                    serves.ReadItem(listId, itemId);
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
                    var listId = int.Parse(Console.ReadLine()!);
                    Console.WriteLine("Enter Id of item:");
                    var itemId = int.Parse(Console.ReadLine()!);
                    Console.WriteLine("Enter ItemName:");
                    var itemName = Console.ReadLine();
                    Console.WriteLine("Enter the Status of the item (Done = true , Still working = false)");
                    var itemIsDone = bool.TryParse(Console.ReadLine(), out var result) && result;

                    serves.UpdateItem(itemId, listId, itemName, itemIsDone);
                    break;
                }
                case 6:
                {
                    serves.RemindMe();
                    Console.WriteLine("Enter Id of List:");
                    var listId = int.Parse(Console.ReadLine()!);
                    Console.WriteLine("Enter Id of item:");
                    var itemId = int.Parse(Console.ReadLine()!);
                    serves.DeleteItem(listId, itemId);
                    break;
                }
                case 7:
                {
                    serves.RemindMe();
                    Console.WriteLine("Enter Id of List:");
                    var listId = int.Parse(Console.ReadLine()!);
                    Console.WriteLine("Enter Id of item:");
                    var itemId = int.Parse(Console.ReadLine()!);
                    serves.MarkAsDone(listId, itemId);
                    break;
                }
            }

            Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
            Console.WriteLine("Do you want" +
                              "\n1-To do another service" +
                              "\n2-To Exit App");
            int.TryParse(Console.ReadLine(), out var appResult);
            if (appResult == 1) continue;

            break;
        }

        Console.ReadLine();
    }
}
