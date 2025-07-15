using SwissPension.Todo.Common;

namespace SwissPension.Todo.CliClient.Helpers;

internal static class ConsolePrinter
{
    public static void PrintCreatedList(int listId, string listName)
    {
        Console.WriteLine(new string('-', 20));
        Console.WriteLine("|{0,-10} | {1,-5} |", "ListId", "ListName");
        Console.WriteLine(new string('-', 20));
        Console.WriteLine("|{0,-10} | {1,-5} |", listId, listName);
        Console.WriteLine(new string('-', 20));
    }

    public static void PrintItem(ReadItemTodoResponse item)
    {
        Console.WriteLine(new string('-', 65));
        Console.WriteLine("{0,-12} | {1,-12} | {2,-8} | {3,-15} | {4,-8}", "ListId", "ListName", "ItemId", "ItemName", "IsDone");
        Console.WriteLine(new string('-', 65));
        Console.WriteLine("{0,-12} | {1,-12} | {2,-8} | {3,-15} | {4,-8}", item.TodoListId, item.ListName, item.Id, item.ItemName, item.IsDone);
        Console.WriteLine(new string('-', 65));
    }

    public static void PrintLists(ListResponse result)
    {
        foreach (var list in result.Lists)
        {
            if (list != null)
            {
                Console.WriteLine($"\n List: {list.ListName} (ID: {list.Id})");
                Console.WriteLine(new string('-', 50));
                Console.WriteLine("{0,-5} | {1,-20} | {2,-6}", "ID", "Item Name", "Done?");
                Console.WriteLine(new string('-', 50));

                foreach (var item in list.Items)
                    Console.WriteLine("{0,-5} | {1,-20} | {2,-6}", item.Id, item.ItemName, item.IsDone);

                Console.WriteLine(new string('-', 50));

                return;
            }

            Console.WriteLine("List not found.");
        }
    }

    public static void PrintUpdatedItem(UpdateItemTodoResponse result) => Console.WriteLine(result is { Id: > 0 } ? "--->>>The Item is updated<<<----" : "Failed to update the item. Please check the ID and try again.");
}
