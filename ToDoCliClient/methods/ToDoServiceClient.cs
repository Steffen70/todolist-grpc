using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using ToDoGrpc;

namespace ToDoCliClient.methods
{
    public class ToDoServiceClient
    {
        // Create Method
        public static void CreateList(string list_name, ToDoGrpc.todoit.todoitClient todoClient)
        {
            todoClient.CreateList(new() { ListName = list_name });
            var result = todoClient.ReadLists(new()).Lists.FirstOrDefault(x => x.ListName == list_name);

            Console.WriteLine(new string('-', 20));
            Console.WriteLine("|{0,-10} | {1,-5} |", "ListId", "ListName");
            Console.WriteLine(new string('-', 20));
            Console.WriteLine("|{0,-10} | {1,-5} |", result.Id, result.ListName);
            Console.WriteLine(new string('-', 20));
        }

        // Add Item 
        public static void AddItem(Int32 list_id,string item_name, todoit.todoitClient todoClient)
        {
            var result = todoClient.ReadLists(new());
            if (result.Lists.FirstOrDefault(x => x.Id == list_id) == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "can't find this one"));

            }
            else
            {
                todoClient.AddItemToList(new()
                {
                    ToDoListId = list_id,
                    ItemName = item_name,
                });
                Console.WriteLine("--->>>>>Item is added!<<<<<---");
            }
        }
        //Read Method
        public static void ReadItem(Int32 list_id, Int32 item_id, ToDoGrpc.todoit.todoitClient todoClient)
        {
            var result = todoClient.ReadItem(new() { Id = item_id, ToDoListId = list_id });
            if (result == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "can't find this one"));

            }
            else
            {
                Console.WriteLine(new string('-', 65));
                Console.WriteLine("{0,-12} | {1,-12} | {2,-8} | {3,-15} | {4,-8}",
                  "ListId", "ListName", "ItemId", "ItemName", "IsDone");
                Console.WriteLine(new string('-', 65));
                Console.WriteLine("{0,-12} | {1,-12} | {2,-8} | {3,-15} | {4,-8}",
                                  result.ToDoListId, result.ListName, result.Id, result.ItemName, result.IsDone);
                Console.WriteLine(new string('-', 65));

            }
        }
        public static void ReadLists(todoit.todoitClient todoClient)
        {
            var result = todoClient.ReadLists(new());
            foreach (var list in result.Lists)
            {
                if (list != null)
                {
                    Console.WriteLine($"\n List: {list.ListName} (ID: {list.Id})");
                    Console.WriteLine(new string('-', 50));
                    Console.WriteLine("{0,-5} | {1,-20} | {2,-6}", "ID", "Item Name", "Done?");
                    Console.WriteLine(new string('-', 50));

                    foreach (var item in list.Items)
                    {
                        Console.WriteLine("{0,-5} | {1,-20} | {2,-6}", item.Id, item.ItemName, item.IsDone);
                    }

                    Console.WriteLine(new string('-', 50));
                }
                else
                {
                    Console.WriteLine("❌ List not found.");
                }
            }
        }
        public static  void UpdateItems(int item_id , int list_id , string item_name , bool is_done , todoit.todoitClient todoClient)
        {
            var result = todoClient.UpdateItem(new() { Id = item_id , ToDoListId = list_id , ItemName = item_name , IsDone =is_done });
            if (result != null)
            {
                Console.WriteLine("--->>>The Item is updated!!!<<<----");
            }
            else
            {
                Console.WriteLine("updating of item is been faild");
            }
        }
        
    }
}
