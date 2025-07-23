using SwissPension.Todo.CliClient.Helpers;

namespace SwissPension.Todo.CliClient;

public class TodoServiceClient(Todo.Common.Todo.TodoClient todoClient)
{
    public void CreateList(string listName)
    {
        todoClient.CreateList(new() { ListName = listName });

        var result = todoClient.ReadLists(new()).Lists.FirstOrDefault(x => x.ListName == listName);

        if (result is not null)
            ConsolePrinter.PrintCreatedList(result.Id, result.ListName);
        else
            Console.WriteLine($"Failed to create list \"{listName}\".");
    }

    public void AddItem(int listId, string itemName)
    {
        var result = todoClient.ReadLists(new());
        if (result.Lists.FirstOrDefault(x => x.Id == listId) == null)
        {
            Console.WriteLine($"List with ID {listId} not found. Item not added.");
            return;
        }

        todoClient.AddItemToList(new()
        {
            TodoListId = listId,
            ItemName = itemName
        });
        Console.WriteLine($"Item \"{itemName}\" has been added to list {listId}.");
    }

    public void ReadItem(int listId, int itemId)
    {
        var result = todoClient.ReadItem(new() { Id = itemId, TodoListId = listId });
        if (string.IsNullOrEmpty(result.ItemName))
        {
            Console.WriteLine($"Item with ID {itemId} was not found in list {listId}.");
            return;
        }

        ConsolePrinter.PrintItem(result);
    }

    public void ReadLists()
    {
        var result = todoClient.ReadLists(new());
        ConsolePrinter.PrintLists(result);
    }

    public void UpdateItem(int itemId, int listId, string itemName, bool isDone)
    {
        var result = todoClient.UpdateItem(new()
        {
            Id = itemId,
            TodoListId = listId,
            ItemName = itemName,
            IsDone = isDone
        });

        ConsolePrinter.PrintUpdatedItem(result);
    }

    public void DeleteItem(int listId, int itemId)
    {
        todoClient.DeleteItem(new()
        {
            TodoListId = listId,
            Id = itemId
        });

        Console.WriteLine($"Item {itemId} has been deleted from list {listId}.");
    }

    // Helper to optionally display lists before certain actions
    public void OptionallyListAll()
    {
        Console.Write("Would you like to display all lists to look up IDs? (y/n): ");
        var showLists = ReadBool();

        if (!showLists) return;

        ReadLists();
        Console.WriteLine();
    }

    public void MarkAsDone(int listId, int itemId)
    {
        todoClient.MarkAsDone(new()
        {
            TodoListId = listId,
            Id = itemId
        });

        Console.WriteLine($"Item {itemId} in list {listId} has been marked as done.");
    }

    public static string PromptNonEmpty(string prompt)
    {
        string input;
        do
        {
            Console.Write(prompt);
            input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                Console.WriteLine("The value cannot be empty. Please try again.");
        } while (string.IsNullOrWhiteSpace(input));

        return input;
    }

    public static int ReadInt()
    {
        int value;

        while (!int.TryParse(Console.ReadLine(), out value)) Console.Write("Invalid number. Please try again: ");

        return value;
    }

    public static int ReadIntInRange(int min, int max)
    {
        var value = ReadInt();
        while (value < min || value > max)
        {
            Console.Write($"Enter a number between {min} and {max}: ");
            value = ReadInt();
        }

        return value;
    }

    public static bool ReadBool()
    {
        while (true)
        {
            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.Y:
                    Console.WriteLine("Yes");
                    return true;
                case ConsoleKey.N:
                    Console.WriteLine("No");
                    return false;
                default:
                    Console.WriteLine();
                    Console.Write("Invalid input. Press 'Y' for Yes or 'N' for No: ");
                    break;
            }
        }
    }

    public static int PromptForListId()
    {
        Console.Write("Enter list ID: ");
        return ReadInt();
    }
}
