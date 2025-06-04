using System.ComponentModel.DataAnnotations;

namespace ToDoGrpc.Models;

public class ToDoList(string listName)
{
    [Key]
    public int Id { get; init; }

    public string ListName { get; init; } = listName;

    public ICollection<ToDoItem> Items { get; init; } = new List<ToDoItem>();
}