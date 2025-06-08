using Microsoft.EntityFrameworkCore;
using ToDoGrpc.Models;

namespace ToDoGrpc.Data;

public sealed class AppDbContext : DbContext
{
        public AppDbContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
             options.UseSqlServer("Server=ENG-NADER\\SQL112000;Database=ToDoDataBase;Trusted_Connection=True;Encrypt=False;");
            //options.UseSqlite($"Data Source={Path.Combine(AppContext.BaseDirectory, "development.db")}");
        }
        public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();
        public DbSet<ToDoList> ToDoLists => Set<ToDoList>();

}

