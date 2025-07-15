using Microsoft.EntityFrameworkCore;
using SwissPension.Todo.Server.Models;

namespace SwissPension.Todo.Server.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext() => Database.EnsureCreated();

    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={Path.Combine(AppContext.BaseDirectory, "todo_lists.db")}");

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<TodoList> TodoLists => Set<TodoList>();
}
