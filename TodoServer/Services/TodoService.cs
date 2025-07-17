using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using SwissPension.Todo.Server.Data;
using SwissPension.Todo.Server.Models;
using SwissPension.Todo.Common;

namespace SwissPension.Todo.Server.Services;

public class TodoService(AppDbContext dbContext) : Todo.Common.Todo.TodoBase
{
    public override async Task<CreateListTodoResponse> CreateList(CreateListTodoRequest request, ServerCallContext context)
    {
        if (request.ListName == string.Empty)
            throw new RpcException(new(StatusCode.InvalidArgument, "List name cannot be empty"));

        var todoList = new TodoList(request.ListName);

        await dbContext.AddAsync(todoList);

        await dbContext.SaveChangesAsync();

        return await Task.FromResult(new CreateListTodoResponse
        {
            Id = todoList.Id
        });
    }

    public override async Task<DeleteListTodoResponse> DeleteList(DeleteListTodoRequest request, ServerCallContext context)
    {
        var todoList = await dbContext.TodoLists.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == request.Id);

        if (todoList == null)
            throw new RpcException(new(StatusCode.NotFound, $"Todo list with ID {request.Id} not found"));

        dbContext.TodoLists.Remove(todoList);

        await dbContext.SaveChangesAsync();

        return await Task.FromResult(new DeleteListTodoResponse
        {
            Id = todoList.Id
        });
    }

    public override async Task<AddItemTodoResponse> AddItemToList(AddItemTodoRequest request, ServerCallContext context)
    {
        if (request.ItemName == string.Empty)
            throw new RpcException(new(StatusCode.InvalidArgument, "Item name cannot be empty"));

        if (!await dbContext.TodoLists.AnyAsync(x => x.Id == request.TodoListId))
            throw new RpcException(new(StatusCode.NotFound, $"Todo list with ID {request.TodoListId} not found"));

        var item = new TodoItem(request.ItemName, request.TodoListId);

        await dbContext.AddAsync(item);

        await dbContext.SaveChangesAsync();

        return new()
        {
            TodoListId = item.TodoListId,
            Id = item.Id
        };
    }

    public override async Task<ReadItemTodoResponse> ReadItem(ReadItemTodoRequest request, ServerCallContext context)
    {
        var item = await dbContext.TodoItems.Include(x => x.TodoList).FirstOrDefaultAsync(x => x.Id == request.Id && x.TodoListId == request.TodoListId);

        if (item == null)
            throw new RpcException(new(StatusCode.NotFound, $"Todo item with ID {request.Id} not found in list {request.TodoListId}"));

        return await Task.FromResult(new ReadItemTodoResponse
        {
            TodoListId = item.TodoListId,
            ListName = item.TodoList.ListName,
            Id = item.Id,
            ItemName = item.ItemName,
            IsDone = item.IsDone
        });
    }

    public override async Task<UpdateItemTodoResponse> UpdateItem(UpdateItemTodoRequest request, ServerCallContext context)
    {
        var item = await dbContext.TodoItems.Include(x => x.TodoList).FirstOrDefaultAsync(x => x.Id == request.Id && x.TodoListId == request.TodoListId);

        Console.WriteLine($"Searching for Item with Id = {request.Id}, ListId = {request.TodoListId}");

        if (item == null)
            throw new RpcException(new(StatusCode.NotFound, $"Todo item with ID {request.Id} not found in list {request.TodoListId}"));

        item.ItemName = request.ItemName;
        item.IsDone = request.IsDone;

        await dbContext.SaveChangesAsync();

        return new()
        {
            Id = request.Id,
            TodoListId = request.TodoListId
        };
    }

    public override async Task<DeleteItemTodoResponse> DeleteItem(DeleteItemTodoRequest request, ServerCallContext context)
    {
        var item = await dbContext.TodoItems.Include(x => x.TodoList).FirstOrDefaultAsync(x => x.Id == request.Id && x.TodoListId == request.TodoListId);

        if (item == null)
            throw new RpcException(new(StatusCode.NotFound, $"Todo item with ID {request.Id} not found in list {request.TodoListId}"));

        dbContext.TodoItems.Remove(item);

        await dbContext.SaveChangesAsync();

        return await Task.FromResult(new DeleteItemTodoResponse
        {
            TodoListId = item.TodoListId,
            Id = item.Id
        });
    }

    public override async Task<ListResponse> ReadLists(Empty request, ServerCallContext context)
    {
        var response = new ListResponse();
        var lists = await dbContext.TodoLists.Include(x => x.Items).ToListAsync();

        foreach (var l in lists)
        {
            var list = new ListTodoItem
            {
                ListName = l.ListName,
                Id = l.Id
            };

            foreach (var i in l.Items)
                list.Items.Add(new ReadItemTodoResponse
                {
                    TodoListId = i.TodoListId,
                    ListName = i.TodoList.ListName,
                    Id = i.Id,
                    ItemName = i.ItemName,
                    IsDone = i.IsDone
                });

            response.Lists.Add(list);
        }

        return response;
    }

    public override async Task<MarkAsDoneTodoResponse> MarkAsDone(MarkAsDoneTodoRequest request, ServerCallContext context)
    {
        var item = await dbContext.TodoItems.Include(x => x.TodoList).FirstOrDefaultAsync(x => x.Id == request.Id && x.TodoListId == request.TodoListId);

        if (item == null)
            throw new RpcException(new(StatusCode.NotFound, $"Todo item with ID {request.Id} not found in list {request.TodoListId}"));

        item.IsDone = true;

        await dbContext.SaveChangesAsync();

        return new()
        {
            TodoListId = item.TodoListId,
            Id = item.Id,
            ItemName = item.ItemName,
            IsDone = true
        };
    }
}
