using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Data.Configurations
{
    public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            //Base Properties
            builder.Property(ti => ti.Title).IsRequired().HasMaxLength(50);
            builder.Property(ti => ti.Description).IsRequired().HasMaxLength(255);
            builder.Property(ti => ti.CreatedDate).IsRequired().HasDefaultValueSql("GETUTCDATE()").ValueGeneratedOnAdd();

            //Navigation Properties
            builder.Property(ti => ti.UserId).IsRequired();
            builder.Property(ti => ti.StatusId).IsRequired();

            //One to Many Relationships
            builder.HasOne(ti => ti.User).WithMany(tmu => tmu.TaskItems).HasForeignKey(ti => ti.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(ti => ti.Status).WithMany(tis => tis.TaskItems).HasForeignKey(ti => ti.StatusId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}