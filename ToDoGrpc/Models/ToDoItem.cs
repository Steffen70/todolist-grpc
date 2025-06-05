using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoGrpc.Models;

public class ToDoItem(string itemName, int toDoListId)
{
    [Key]
    public int Id { get; init; }

    public string ItemName { get; set; } = itemName;
    public bool IsDone { get; set; }

    [ForeignKey("ToDoList")]
    public int ToDoListId { get; init; } = toDoListId;

    public ToDoList ToDoList { get; init; } = null!;
}