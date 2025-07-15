using System.Security.Cryptography.X509Certificates;
using Grpc.Net.Client;
using static SwissPension.Todo.CliClient.TodoServiceClient;

namespace SwissPension.Todo.CliClient;

public static class Program
{
    internal static void Main()
    {
        var baseAddress = new Uri("https://localhost:8443");
        using var rootCa = new X509Certificate2(Path.Combine(AppContext.BaseDirectory, "cert", "root_ca.crt"));

        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (_, cert, chain, _) =>
        {
            chain!.ChainPolicy.ExtraStore.Add(rootCa);
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            return chain.Build(cert!);
        };

        var httpClient = new HttpClient(handler);
        var channel = GrpcChannel.ForAddress(baseAddress, new() { HttpClient = httpClient });
        var todoClient = new Common.Todo.TodoClient(channel);
        var service = new TodoServiceClient(todoClient);

        while (true)
        {
            Console.Clear();
            Console.WriteLine("==================================================");
            Console.WriteLine("                TODO-LIST APP MENU                ");
            Console.WriteLine("==================================================");

            Console.WriteLine("SELECT AN OPERATION:");
            Console.WriteLine("    1  Create new list");
            Console.WriteLine("    2  Add item to list");
            Console.WriteLine("    3  Read an item");
            Console.WriteLine("    4  List all lists with items");
            Console.WriteLine("    5  Update an item");
            Console.WriteLine("    6  Delete an item");
            Console.WriteLine("    7  Mark item as done");
            Console.WriteLine("    8  Exit");
            Console.Write("Command> ");

            var choice = ReadIntInRange(1, 8);
            Console.WriteLine();

            if (choice == 8)
                break;

            switch (choice)
            {
                case 1:
                    var listName = PromptNonEmpty("Enter list name: ");
                    service.CreateList(listName);
                    break;

                case 2:
                    service.OptionallyListAll();
                    var targetListId = PromptForListId();
                    var itemName = PromptNonEmpty("Enter item name: ");
                    service.AddItem(targetListId, itemName);
                    break;

                case 3:
                    var readListId = PromptForListId();
                    Console.Write("Enter item ID: ");
                    var readItemId = ReadInt();
                    service.ReadItem(readListId, readItemId);
                    break;

                case 4:
                    service.ReadLists();
                    break;

                case 5:
                    service.OptionallyListAll();
                    var updateListId = PromptForListId();
                    Console.Write("Enter item ID: ");
                    var updateItemId = ReadInt();
                    var newName = PromptNonEmpty("Enter new item name: ");
                    Console.Write("Is the item done? (y/n): ");
                    var done = ReadBool();
                    service.UpdateItem(updateItemId, updateListId, newName, done);
                    break;

                case 6:
                    service.OptionallyListAll();
                    var delListId = PromptForListId();
                    Console.Write("Enter item ID to delete: ");
                    var delItemId = ReadInt();
                    service.DeleteItem(delListId, delItemId);
                    break;

                case 7:
                    service.OptionallyListAll();
                    var markListId = PromptForListId();
                    Console.Write("Enter item ID to mark done: ");
                    var markItemId = ReadInt();
                    service.MarkAsDone(markListId, markItemId);
                    break;
            }

            Console.WriteLine();
            Console.Write("Press ENTER to return to main menu...");
            Console.ReadLine();
        }
    }
}
