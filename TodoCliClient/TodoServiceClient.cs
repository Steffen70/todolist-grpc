using Grpc.Core;
using SwissPension.Todo.CliClient.Helpers;

namespace SwissPension.Todo.CliClient;

public interface ITodoServiceClient
{
    void CreateList(string listName);
    void AddItem(int listId, string itemName);
    void ReadItem(int listId, int itemId);
    void ReadLists();
    void UpdateItem(int itemId, int listId, string itemName, bool isDone);
    void DeleteItem(int listId, int itemId);
    void MarkAsDone(int listId, int itemId);
    void RemindMe();
}

public class TodoServiceClient(Todo.Common.Todo.TodoClient todoClient) : ITodoServiceClient
{
    // Create Method
    public void CreateList(string listName)
    {
        todoClient.CreateList(new() { ListName = listName });

        var result = todoClient.ReadLists(new()).Lists.FirstOrDefault(x => x.ListName == listName);
        ConsolePrinter.PrintCreatedList(result.Id, result.ListName);
    }

    // Add Item 
    public void AddItem(int listId, string itemName)
    {
        var result = todoClient.ReadLists(new());
        if (result.Lists.FirstOrDefault(x => x.Id == listId) == null)
            throw new RpcException(new(StatusCode.InvalidArgument, "can't find this one"));

        todoClient.AddItemToList(new()
        {
            TodoListId = listId,
            ItemName = itemName
        });
        Console.WriteLine($"✅ Item \"{itemName}\" was added to list ID {listId}.");
    }

    //Read Method
    public void ReadItem(int listId, int itemId)
    {
        var result = todoClient.ReadItem(new() { Id = itemId, TodoListId = listId });
        if (result.ItemName == null)
            throw new RpcException(new(StatusCode.InvalidArgument, "can't find this one"));

        ConsolePrinter.PrintItem(result);
    }

    public void ReadLists()
    {
        var result = todoClient.ReadLists(new());

        ConsolePrinter.PrintLists(result);
    }

    public void UpdateItem(int itemId, int listId, string itemName, bool isDone)
    {
        var result = todoClient.UpdateItem(new() { Id = itemId, TodoListId = listId, ItemName = itemName, IsDone = isDone });

        ConsolePrinter.PrintUpdatedItem(result);
    }

    public void DeleteItem(int listId, int itemId)
    {
        ReadLists();
        todoClient.DeleteItem(new()
        {
            TodoListId = listId,
            Id = itemId
        });

        Console.WriteLine($"Item has || ListId:{listId} || ItemId:{itemId} || is deleted!!");
    }

    public void RemindMe()
    {
        Console.WriteLine("Do u want to see the lists to get the id of item and list" +
                          "\n1-if Yes" +
                          "\n2-No u have the ids");

        int.TryParse(Console.ReadLine(), out var ch);

        if (ch == 1) ReadLists();

        Console.WriteLine("Press Enter To Continue:");
        Console.ReadLine();
    }

    public void MarkAsDone(int listId, int itemId)
    {
        todoClient.MarkAsDone(new()
        {
            TodoListId = listId,
            Id = itemId
        });

        Console.WriteLine($"---->>Item has || ListId:{listId} || ItemId:{itemId} || is Marked as Done!!<<----");
    }
}
