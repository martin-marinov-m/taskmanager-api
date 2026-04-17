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
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new TaskItemConfiguration());
            builder.ApplyConfiguration(new TaskItemStatusConfiguration());

            // Adjust default value for SQLite (used in tests with in-memory database)
            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                builder.Entity<TaskItem>()
                    .Property(ti => ti.CreatedDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            }
        }
    }
}
