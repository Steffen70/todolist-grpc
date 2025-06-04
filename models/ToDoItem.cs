using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoGrpc.models
{
    public class ToDoItem
    {
        [Key]
        public int Id { get; set; }
        public string?  ItemName { get; set; }
        public bool IsDone { get; set; }

        [ForeignKey("ToDoList")]
        public int ToDoListId {  get; set; }
        public ToDoList ToDoList { get; set; }

    }
}
