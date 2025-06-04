using System;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ToDoGrpc.Data;
using ToDoGrpc.models;


namespace ToDoGrpc.Services;

public class ToDoService : todoit.todoitBase
{
    private readonly AppDbContext _dbContext;

    public ToDoService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async override Task<CreateListToDoResponse> CreateList(CreateListToDoRequest request , ServerCallContext context)
    {
        if(request.ListName == string.Empty)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ("Enter Vaild Argument")));
        }
        var DoList = new ToDoList()
        {
            ListName = request.ListName,
        };
        await _dbContext.AddAsync(DoList);
        await _dbContext.SaveChangesAsync();
        return await Task.FromResult(new CreateListToDoResponse
        {
            Id = DoList.Id
        });
    }
    public async override Task<AddItemToDoResponse> AddItemToList(AddItemToDoRequest request, ServerCallContext context)
    {
        if (!await _dbContext.ToDoLists.AnyAsync(x => x.Id == request.ToDoListId) || request.ItemName == string.Empty)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Enter vaild Arguements:-"));
        }
        var to_do_item = new ToDoItem()
        {
            ToDoListId = request.ToDoListId,
            ItemName = request.ItemName,
        };
        await _dbContext.AddAsync(to_do_item);
        await _dbContext.SaveChangesAsync();
        return await Task.FromResult(new AddItemToDoResponse
        {
            ToDoListId = to_do_item.ToDoListId,
            Id = to_do_item.Id
        });
    }
    public async override Task<ReadItemToDoResponse> ReadItem (ReadItemToDoRequest request, ServerCallContext context)
    {
        var item = await _dbContext.ToDoItems.Include(x=>x.ToDoList).FirstOrDefaultAsync(x => x.Id == request.Id && x.ToDoListId == request.ToDoListId); 
        if (item == null ) 
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "can't find this one"));        
        }
    
        return await Task.FromResult(new ReadItemToDoResponse
        {
            ToDoListId = item.ToDoListId,
            ListName = item.ToDoList.ListName,
            Id = item.Id,
            ItemName = item.ItemName,
            IsDone = item.IsDone
        });

    }
    public async override Task<UpdateItemToDoResponse> UpdateItem(UpdateItemToDoRequest request,ServerCallContext context)
    {
        var item = await _dbContext.ToDoItems.Include(x=>x.ToDoList).FirstOrDefaultAsync(x=>x.Id == request.Id &&  x.ToDoListId == request.ToDoListId);
        Console.WriteLine($"Searching for Item with Id = {request.Id}, ListId = {request.ToDoListId}");

        if (item == null )

        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Enter Correct Indexes"));
        }
        item.ItemName= request.ItemName;
        item.IsDone= request.IsDone;
        await _dbContext.SaveChangesAsync();
        return await Task.FromResult
            (new UpdateItemToDoResponse()
            {
                Id = request.Id,
                ToDoListId = request.ToDoListId
            });

    }
    public async override Task<DeleteItemToDoResponse> DeleteItem(DeleteItemToDoRequest request , ServerCallContext context)
    {
        var Item = await _dbContext.ToDoItems.Include(x=>x.ToDoList).FirstOrDefaultAsync(x=> x.Id == request.Id && x.ToDoListId == request.ToDoListId);
        if (Item == null )
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Enter Vaild List"));
        }
        _dbContext.ToDoItems.Remove(Item);
        await _dbContext.SaveChangesAsync();
        return await Task.FromResult(new DeleteItemToDoResponse()
        {
            ToDoListId = Item.ToDoListId,
            Id = Item.Id
        });
    }
    public async override  Task<ListResponse> ReadLists(Empty request,ServerCallContext context)
    {
        var response = new ListResponse();
        var Lists = await _dbContext.ToDoLists.Include(x=>x.Items).ToListAsync();
        foreach(var one in Lists)
        {
            var List = new ListToDoItem
            {
                ListName = one.ListName,
                Id = one.Id
            };
            foreach(var two in one.Items)
            {
                List.Items.Add( new ReadItemToDoResponse
                {
                    ToDoListId = two.ToDoListId,
                    ListName=two.ToDoList.ListName,
                    Id = two.Id,
                    ItemName = two.ItemName,
                    IsDone = two.IsDone
                });
            }
            response.Lists.Add(List);
        }
        return response;
    }
    public async override Task<MarkAsDoneToDoResponse> MarkAsDone(MarkAsDoneToDoRequest request,ServerCallContext context)
    {
        var Item = await _dbContext.ToDoItems.Include(x=>x.ToDoList).FirstOrDefaultAsync(x=>x.Id == request.Id && x.ToDoListId == request.ToDoListId);
        if (Item == null)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "The Arguments are not Correct"));
        }
        Item.IsDone = true;
        await _dbContext.SaveChangesAsync();
        return await Task.FromResult(new MarkAsDoneToDoResponse
        {
            ToDoListId = Item.ToDoListId,
            Id = Item.Id,
            ItemName = Item.ItemName,
            IsDone = true
        });

    }
}

