using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;
using TaskManagerAPI.Models.Identity;
using TaskManagerAPI.Repositories;

namespace TaskManagerAPI.Tests
{
    public class TaskItemRepositoryTests
    {
        [Fact]
        public async Task AddAsync_ShouldAddTaskItem()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var repository = await CreateSeededTaskItemRepository(dbContext);
            var ct = CancellationToken.None;

            var taskItem = new TaskItem()
            {
                Title = "AddTest",
                Description = "AddTest",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 1,
                UserId = "developerTestId"
            };

            //Act
            await repository.AddAsync(taskItem, ct);
            await repository.SaveChangesAsync(ct);

            //Assert
            var result = await dbContext.TaskItems.Include(ti => ti.Status).Include(ti => ti.User).FirstOrDefaultAsync(ti => ti.Id == taskItem.Id);

            Assert.NotNull(result);
            Assert.NotNull(result.Status);
            Assert.NotNull(result.User);
            Assert.Equal(taskItem.Id, result.Id);
            Assert.Equal("AddTest", result.Title);
            Assert.Equal("AddTest", result.Description);
            Assert.Equal("developerTestId", result.UserId);
            Assert.Equal(1, result.StatusId);
        }

        [Fact]
        public async Task GetAllAsync_WhenTaskItemsExist_ShouldReturnTaskItems()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var repository = await CreateSeededTaskItemRepository(dbContext);

            var ct = CancellationToken.None;

            var taskItems = new List<TaskItem>()
            {
              new TaskItem()
            {
                Title = "GetAllTest-developer",
                Description = "GetAllTest-developer",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 1,
                UserId = "developerTestId"
            },
              new TaskItem()
            {
                Title = "GetAllTest-teamLedaer",
                Description = "GetAllTest-teamLedaer",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 1,
                UserId = "teamLeaderTestId"
            },
              new TaskItem()
            {
                Title = "GetAllTest-admin",
                Description = "GetAllTest-admin",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 1,
                UserId = "adminTestId"
            }
            };

            await dbContext.TaskItems.AddRangeAsync(taskItems);
            await dbContext.SaveChangesAsync();

            //Act
            var result = await repository.GetAllAsync(ct);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Contains(result, r => r.Title == "GetAllTest-developer");
            Assert.Contains(result, r => r.Title == "GetAllTest-teamLedaer");
            Assert.Contains(result, r => r.Title == "GetAllTest-admin");
        }

        [Fact]
        public async Task GetAllAsync_WhenTaskItemsDoesNotExist_ShouldReturnEmptyList()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var repository = CreateTaskItemRepository(dbContext);

            var ct = CancellationToken.None;

            //Act
            var result = await repository.GetAllAsync(ct);

            //Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenTaskItemExists_ShouldReturnTaskItem()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var repository = await CreateSeededTaskItemRepository(dbContext);
            var ct = CancellationToken.None;

            var taskItem = new TaskItem()
            {
                Title = "GetByIdTest",
                Description = "GetByIdTest",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 1,
                UserId = "developerTestId"
            };

            await dbContext.TaskItems.AddAsync(taskItem);
            await dbContext.SaveChangesAsync();
            //Act
            var result = await repository.GetByIdAsync(taskItem.Id, ct);

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Status);
            Assert.NotNull(result.User);
            Assert.Equal(taskItem.Id, result.Id);
            Assert.Equal("GetByIdTest", result.Title);
            Assert.Equal("GetByIdTest", result.Description);
            Assert.Equal(1, result.StatusId);
            Assert.Equal("developerTestId", result.UserId);
        }

        [Fact]
        public async Task GetByIdAsync_WhenTaskItemDoesNotExist_ShouldReturnNull()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var repository = CreateTaskItemRepository(dbContext);
            var ct = CancellationToken.None;

            //Act
            var result = await repository.GetByIdAsync(int.MaxValue, ct);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ExistsAsync_WhenTaskItemExists_ShouldReturnTrue()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var repository = await CreateSeededTaskItemRepository(dbContext);
            var ct = CancellationToken.None;

            var taskItem = new TaskItem()
            {
                Title = "ExistsTest",
                Description = "ExistsTest",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 1,
                UserId = "developerTestId"
            };

            await dbContext.TaskItems.AddAsync(taskItem);
            await dbContext.SaveChangesAsync();

            //Act
            var result = await repository.ExistsAsync(taskItem.Id, ct);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_WhenTaskItemDoesNotExist_ShouldReturnFalse()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var repository = CreateTaskItemRepository(dbContext);
            var ct = CancellationToken.None;

            //Act
            var result = await repository.ExistsAsync(int.MaxValue, ct);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Update_ShouldUpdateDetachedTaskItem()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var repository = await CreateSeededTaskItemRepository(dbContext);

            var ct = CancellationToken.None;

            var taskItem = new TaskItem()
            {
                Title = "UpdateTest",
                Description = "UpdateTest",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 1,
                UserId = "developerTestId"
            };

            await dbContext.TaskItems.AddAsync(taskItem);
            await dbContext.SaveChangesAsync();

            //Act
            dbContext.Entry(taskItem).State = EntityState.Detached;

            taskItem.Title = "UpdateTestupdate";
            taskItem.Description = "UpdateTestupdate";
            taskItem.DueDate = DateTime.UtcNow.AddDays(1);
            taskItem.StatusId = 2;

            repository.Update(taskItem);
            await repository.SaveChangesAsync(ct);

            using var newDbContext = CreateDbContext(connection);

            var result = await newDbContext.TaskItems.Include(ti => ti.Status).Include(ti => ti.User).FirstOrDefaultAsync(ti => ti.Id == taskItem.Id);

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Status);
            Assert.NotNull(result.User);
            Assert.Equal(taskItem.Id, result.Id);
            Assert.Equal("UpdateTestupdate", result.Title);
            Assert.Equal("UpdateTestupdate", result.Description);
            Assert.Equal(taskItem.DueDate, result.DueDate);
            Assert.Equal(2, result.StatusId);
        }

        [Fact]
        public async Task DeleteByIdAsync_WhenTaskItemExists_ShouldReturnTrue()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var repository = await CreateSeededTaskItemRepository(dbContext);
            var ct = CancellationToken.None;

            var taskItem = new TaskItem()
            {
                Title = "DeleteByIdTest",
                Description = "DeleteByIdTest",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 3,
                UserId = "developerTestId"
            };

            dbContext.TaskItems.Add(taskItem);
            await dbContext.SaveChangesAsync();

            //Act
            var resultSucceded = await repository.DeleteByIdAsync(taskItem.Id, ct);
            await repository.SaveChangesAsync(ct);

            var deleteResult = await dbContext.TaskItems.FirstOrDefaultAsync(x => x.Id == taskItem.Id);

            //Assert
            Assert.True(resultSucceded);
            Assert.Null(deleteResult);
        }

        [Fact]
        public async Task DeleteByIdAsync_WhenTaskItemDoesNotExist_ShouldReturnFalse()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var repository = CreateTaskItemRepository(dbContext);
            var ct = CancellationToken.None;

            //Act
            var result = await repository.DeleteByIdAsync(int.MaxValue, ct);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteByEntity_WhenTaskItemExists_ShouldDeleteTaskItem()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var taskItemRepository = await CreateSeededTaskItemRepository(dbContext);
            var ct = CancellationToken.None;

            var taskItem = new TaskItem()
            {
                Title = "DeleteEntityTest",
                Description = "DeleteEntityTest",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 3,
                UserId = "developerTestId"
            };

            dbContext.TaskItems.Add(taskItem);
            await dbContext.SaveChangesAsync();

            //Act
            taskItemRepository.DeleteByEntity(taskItem);
            await taskItemRepository.SaveChangesAsync(ct);

            var result = await dbContext.TaskItems.FirstOrDefaultAsync(ti => ti.Id == taskItem.Id);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteByEntity_WhenTaskItemDoesNotExist_ShouldNotDeleteTaskItem()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var taskItemRepository = await CreateSeededTaskItemRepository(dbContext);
            var ct = CancellationToken.None;

            var taskItem = new TaskItem()
            {
                Title = "DeleteByEntityTest",
                Description = "DeleteByEntityTest",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 3,
                UserId = "developerTestId"
            };

            dbContext.TaskItems.Add(taskItem);
            await dbContext.SaveChangesAsync();

            //Act
            // Entity is still in DB because SaveChanges was not called
            taskItemRepository.DeleteByEntity(taskItem);
            var result = await dbContext.TaskItems.FirstOrDefaultAsync(ti => ti.Id == taskItem.Id);

            //Assert
            Assert.NotNull(result);
        }

        private SqliteConnection CreateConnection()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            return connection;
        }

        private TaskManagerDbContext CreateDbContext(SqliteConnection connection)
        {
            var options = new DbContextOptionsBuilder<TaskManagerDbContext>().UseSqlite(connection).Options;

            var dbContext = new TaskManagerDbContext(options);

            dbContext.Database.EnsureCreated();

            return dbContext;
        }

        private TaskItemRepository CreateTaskItemRepository(TaskManagerDbContext dbContext)
        {
            return new TaskItemRepository(dbContext);
        }

        private async Task<TaskItemRepository> CreateSeededTaskItemRepository(TaskManagerDbContext dbContext)
        {
            await SeedData(dbContext);
            return CreateTaskItemRepository(dbContext);
        }

        private async Task SeedData(TaskManagerDbContext dbContext)
        {
            if (!await dbContext.TaskItemStatuses.AnyAsync())
            {
                var statuses = new List<TaskItemStatus>
            {
                new TaskItemStatus
                {
                    Id = 1,
                    Name = "New"
                },
                new TaskItemStatus
                {
                    Id = 2,
                    Name = "InProgress"
                },
                new TaskItemStatus
                {
                    Id = 3,
                    Name = "Completed"
                },
            };

                await dbContext.TaskItemStatuses.AddRangeAsync(statuses);
                await dbContext.SaveChangesAsync();
            }

            if (!dbContext.Users.Any())
            {
                var users = new List<TaskManagerUser>
            {
                new TaskManagerUser
                {
                    Id = "adminTestId",
                    Email = "adminTest@taskmanager.com",
                    UserName = "adminTest@taskmanager.com"
                },

                new TaskManagerUser
                {
                    Id = "teamLeaderTestId",
                    Email = "teamLeaderTest@taskmanager.com",
                    UserName = "teamLeaderTest@taskmanager.com"
                },

                 new TaskManagerUser
                {
                    Id = "developerTestId",
                    Email = "developerTest@taskmanager.com",
                    UserName = "developerTest@taskmanager.com"
                },
            };

                await dbContext.Users.AddRangeAsync(users);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}