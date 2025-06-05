using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using ToDoGrpc.models;

namespace ToDoGrpc.Data
{
    public sealed class AppDbContext : DbContext
    {
        public AppDbContext()
        {
            Database.EnsureCreated();
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
             options.UseSqlServer(ConnectionS.SqlConStr);
            //options.UseSqlite($"Data Source={Path.Combine(AppContext.BaseDirectory, "development.db")}");
        }
        public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();
        public DbSet<ToDoList> ToDoLists => Set<ToDoList>();
    }
}
    
