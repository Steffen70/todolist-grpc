using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SwissPension.Todo.Server.Models;

public class TodoItem(string itemName, int todoListId)
{
    [Key]
    public int Id { get; private init; }

    [MaxLength(100)]
    public string ItemName { get; set; } = itemName;

    public bool IsDone { get; set; }

    [ForeignKey("TodoList")]
    public int TodoListId { get; private init; } = todoListId;

    public TodoList TodoList { get; private init; } = null!;
}
