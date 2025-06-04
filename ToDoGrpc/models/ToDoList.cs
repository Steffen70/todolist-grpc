using System.ComponentModel.DataAnnotations;

namespace ToDoGrpc.models
{
    public class ToDoList
    {
        [Key]
        public int Id { get; set; }
        public string? ListName { get; set; }

        public ICollection<ToDoItem> Items { get; set; } = new List<ToDoItem>();
        
    }
}
