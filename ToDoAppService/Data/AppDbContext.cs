using Microsoft.EntityFrameworkCore;
using ToDoAppService.Models;

namespace ToDoAppService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();
    }
}
