using System.ComponentModel.DataAnnotations;

namespace SwissPension.Todo.Server.Models;

public class TodoList(string listName)
{
    [Key]
    public int Id { get; private init; }

    [MaxLength(100)]
    public string ListName { get; private init; } = listName;

    public List<TodoItem> Items { get; private init; } = [];
}
