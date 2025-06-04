using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using ToDoGrpc.models;

namespace ToDoGrpc.Data
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(ConnectionS.SqlConStr);

        }
        public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();
        public DbSet<ToDoList> ToDoLists => Set<ToDoList>();



    }
}
    
