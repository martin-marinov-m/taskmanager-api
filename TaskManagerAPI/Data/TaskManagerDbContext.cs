using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data.Configurations;
using TaskManagerAPI.Models;
using TaskManagerAPI.Models.Identity;

namespace TaskManagerAPI.Data
{
    public class TaskManagerDbContext : IdentityDbContext<TaskManagerUser>
    {
        public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options) : base(options) { }

        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<TaskItemStatus> TaskItemStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new TaskItemConfiguration());
            builder.ApplyConfiguration(new TaskItemStatusConfiguration());
            base.OnModelCreating(builder);
        }
    }
}
