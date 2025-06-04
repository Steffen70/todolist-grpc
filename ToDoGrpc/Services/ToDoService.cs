using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ToDoGrpc.Data;
using ToDoGrpc.Models;

namespace ToDoGrpc.Services;

public class ToDoService(AppDbContext dbContext) : todoit.todoitBase
{
    public override async Task<CreateListToDoResponse> CreateList(CreateListToDoRequest request, ServerCallContext context)
    {
        if (request.ListName == string.Empty) throw new RpcException(new(StatusCode.InvalidArgument, "Enter valid Argument"));
        var toDoList = new ToDoList(request.ListName);
        await dbContext.AddAsync(toDoList);
        await dbContext.SaveChangesAsync();
        return await Task.FromResult(new CreateListToDoResponse
        {
            Id = toDoList.Id
        });
    }

    public override async Task<AddItemToDoResponse> AddItemToList(AddItemToDoRequest request, ServerCallContext context)
    {
        if (!await dbContext.ToDoLists.AnyAsync(x => x.Id == request.ToDoListId) || request.ItemName == string.Empty)
            // TODO: Provide a useful exception message
            throw new RpcException(new(StatusCode.InvalidArgument, "Enter valid Arguments"));

        var toDoItem = new ToDoItem(request.ItemName, request.ToDoListId);
        await dbContext.AddAsync(toDoItem);
        await dbContext.SaveChangesAsync();
        return await Task.FromResult(new AddItemToDoResponse
        {
            ToDoListId = toDoItem.ToDoListId,
            Id = toDoItem.Id
        });
    }

    public override async Task<ReadItemToDoResponse> ReadItem(ReadItemToDoRequest request, ServerCallContext context)
    {
        var item = await dbContext.ToDoItems.Include(x => x.ToDoList).FirstOrDefaultAsync(x => x.Id == request.Id && x.ToDoListId == request.ToDoListId);
        if (item == null) throw new RpcException(new(StatusCode.InvalidArgument, "Can't find this one"));

        return await Task.FromResult(new ReadItemToDoResponse
        {
            ToDoListId = item.ToDoListId,
            ListName = item.ToDoList.ListName,
            Id = item.Id,
            ItemName = item.ItemName,
            IsDone = item.IsDone
        });
    }

    public override async Task<UpdateItemToDoResponse> UpdateItem(UpdateItemToDoRequest request, ServerCallContext context)
    {
        var item = await dbContext.ToDoItems.Include(x => x.ToDoList).FirstOrDefaultAsync(x => x.Id == request.Id && x.ToDoListId == request.ToDoListId);
        Console.WriteLine($"Searching for Item with Id = {request.Id}, ListId = {request.ToDoListId}");

        if (item == null) throw new RpcException(new(StatusCode.InvalidArgument, "Enter Correct Indexes"));
        item.ItemName = request.ItemName;
        item.IsDone = request.IsDone;
        await dbContext.SaveChangesAsync();
        return await Task.FromResult
        (new UpdateItemToDoResponse
        {
            Id = request.Id,
            ToDoListId = request.ToDoListId
        });
    }

    public override async Task<DeleteItemToDoResponse> DeleteItem(DeleteItemToDoRequest request, ServerCallContext context)
    {
        var item = await dbContext.ToDoItems.Include(x => x.ToDoList).FirstOrDefaultAsync(x => x.Id == request.Id && x.ToDoListId == request.ToDoListId);
        if (item == null) throw new RpcException(new(StatusCode.InvalidArgument, "Enter Vaild List"));
        dbContext.ToDoItems.Remove(item);
        await dbContext.SaveChangesAsync();
        return await Task.FromResult(new DeleteItemToDoResponse
        {
            ToDoListId = item.ToDoListId,
            Id = item.Id
        });
    }

    public override async Task<ListResponse> ReadLists(Empty request, ServerCallContext context)
    {
        var response = new ListResponse();
        var lists = await dbContext.ToDoLists.Include(x => x.Items).ToListAsync();
        foreach (var one in lists)
        {
            var list = new ListToDoItem
            {
                ListName = one.ListName,
                Id = one.Id
            };
            foreach (var two in one.Items)
                list.Items.Add(new ReadItemToDoResponse
                {
                    ToDoListId = two.ToDoListId,
                    ListName = two.ToDoList.ListName,
                    Id = two.Id,
                    ItemName = two.ItemName,
                    IsDone = two.IsDone
                });
            response.Lists.Add(list);
        }

        return response;
    }

    public override async Task<MarkAsDoneToDoResponse> MarkAsDone(MarkAsDoneToDoRequest request, ServerCallContext context)
    {
        var item = await dbContext.ToDoItems.Include(x => x.ToDoList).FirstOrDefaultAsync(x => x.Id == request.Id && x.ToDoListId == request.ToDoListId);
        if (item == null) throw new RpcException(new(StatusCode.InvalidArgument, "The Arguments are not Correct"));
        item.IsDone = true;
        await dbContext.SaveChangesAsync();
        return await Task.FromResult(new MarkAsDoneToDoResponse
        {
            ToDoListId = item.ToDoListId,
            Id = item.Id,
            ItemName = item.ItemName,
            IsDone = true
        });
    }
}