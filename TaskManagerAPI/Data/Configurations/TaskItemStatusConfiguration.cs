using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Data.Configurations
{
    public class TaskItemStatusConfiguration : IEntityTypeConfiguration<TaskItemStatus>
    {
        public void Configure(EntityTypeBuilder<TaskItemStatus> builder)
        {
            builder.Property(tis => tis.Name).IsRequired().HasMaxLength(50);

            builder.HasData(
                new TaskItemStatus
                {
                    Id = 1,
                    Name = "New",
                },
                new TaskItemStatus
                {
                    Id = 2,
                    Name = "InProgress",
                },
                new TaskItemStatus
                {
                    Id = 3,
                    Name = "Completed",
                });


        }
    }
}
