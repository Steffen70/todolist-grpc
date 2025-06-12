using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using ToDoGrpc;
using static ToDoGrpc.todoit;

namespace ToDoCliClient.methods
{
    public interface IToDoServiceClient
    {
        void CreateList(string list_name);
        void AddItem(int list_id, string item_name);
        void ReadItem(int list_id, int item_id);
        void ReadLists();
        void UpdateItem(int item_id, int list_id, string item_name, bool is_done);
        void DeleteItem(int list_id, int item_id);
        void MarkAsDone(int list_id, int item_id);
        void RemindMe();
    }
    public class ToDoServiceClient : IToDoServiceClient
    {
        private readonly todoit.todoitClient _todoClient;

        public ToDoServiceClient(todoit.todoitClient todoClient)
        {
            _todoClient = todoClient;
        }
        // Create Method
        public  void CreateList(string list_name)
        {
            _todoClient.CreateList(new() { ListName = list_name });
            
            var result = _todoClient.ReadLists(new()).Lists.FirstOrDefault(x => x.ListName == list_name);

            Console.WriteLine(new string('-', 20));
            Console.WriteLine("|{0,-10} | {1,-5} |", "ListId", "ListName");
            Console.WriteLine(new string('-', 20));
            Console.WriteLine("|{0,-10} | {1,-5} |", result.Id, result.ListName);
            Console.WriteLine(new string('-', 20));
        }

        // Add Item 
        public  void AddItem(Int32 list_id,string item_name)
        {
            var result = _todoClient.ReadLists(new());
            if (result.Lists.FirstOrDefault(x => x.Id == list_id) == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "can't find this one"));

            }
            else
            {
                _todoClient.AddItemToList(new()
                {
                    ToDoListId = list_id,
                    ItemName = item_name,
                });
                Console.WriteLine($"✅ Item \"{item_name}\" was added to list ID {list_id}.");

            }
        }
        //Read Method
        public void ReadItem(Int32 list_id, Int32 item_id)
        {
            var result = _todoClient.ReadItem(new() { Id = item_id, ToDoListId = list_id });
            if (result.ItemName == null)
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
        public  void ReadLists()
        {
            var result = _todoClient.ReadLists(new());
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
        public   void UpdateItem(int item_id , int list_id , string item_name , bool is_done)
        {
            var result = _todoClient.UpdateItem(new() { Id = item_id , ToDoListId = list_id , ItemName = item_name , IsDone =is_done });
            if (result != null)
            {
                Console.WriteLine("--->>>The Item is updated!!!<<<----");
            }
            else
            {
                Console.WriteLine("❌ Failed to update the item. Please check the ID and try again.");

            }
        }
        public  void DeleteItem(int list_id, int item_id)
        {
            this.ReadLists();
            _todoClient.DeleteItem(new()
            {
                ToDoListId = list_id,
                Id = item_id
            });

            Console.WriteLine($"Item has || ListId:{list_id} || ItemId:{item_id} || is deleted!!");
        }
        public  void RemindMe()
        {
            Console.WriteLine("Do u want to see the lists to get the id of item and list" +
                        "\n1-if Yes\n2-No u have the ids");
            int.TryParse(Console.ReadLine(), out int ch);
            if (ch == 1)
            {
                this.ReadLists();
                Console.WriteLine("Press Enter To Continue:");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Press Enter To Continue:");
                Console.ReadLine();
            }
        }
        public  void MarkAsDone(int list_id, int item_id)
        {
            _todoClient.MarkAsDone(new()
            {
                ToDoListId = list_id,
                Id = item_id
            });
            Console.WriteLine($"---->>Item has || ListId:{list_id} || ItemId:{item_id} || is Marked as Done!!<<----");
        }


    }
}
