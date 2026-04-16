using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;
using TaskManagerAPI.Repositories;

namespace TaskManagerAPI.Tests
{
    public class TaskItemStatusRepositoryTests
    {
        [Fact]
        public async Task GetByIdAsync_WhenTaskItemStatusExists_ShouldReturnTaskItemStatus()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var repository = CreateTaskItemStatusRepository(dbContext);

            var ct = CancellationToken.None;

            var status = new TaskItemStatus
            {
                Name = "GetByIdStatusTest"
            };

            await dbContext.TaskItemStatuses.AddAsync(status);
            await dbContext.SaveChangesAsync();

            //Act

            var result = await repository.GetByIdAsync(status.Id, ct);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(status.Id, result.Id);
            Assert.Equal("GetByIdStatusTest", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WhenTaskItemStatusDoesNotExists_ShouldReturnNull()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var repository = CreateTaskItemStatusRepository(dbContext);

            var ct = CancellationToken.None;

            //Act
            var result = await repository.GetByIdAsync(999, ct);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_WhenTaskItemStatusesExist_ShouldReturnTaskItemStatuses()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var repository = CreateTaskItemStatusRepository(dbContext);

            var ct = CancellationToken.None;

            var statuses = new List<TaskItemStatus>()
            {
                new TaskItemStatus
            {
                Name = "GetAllStatusesTest1"
            },
                new TaskItemStatus
            {
                Name = "GetAllStatusesTest2"
            },
                new TaskItemStatus
            {
                Name = "GetAllStatusesTest3"
            },
                new TaskItemStatus
            {
                Name = "GetAllStatusesTest4"
            },
            };

            await dbContext.TaskItemStatuses.AddRangeAsync(statuses);
            await dbContext.SaveChangesAsync();

            //Act
            var result = await repository.GetAllAsync(ct);

            //Assert
            Assert.NotNull(result);


            Assert.True(result.Count() >= 4);
            Assert.NotEmpty(result);
            Assert.Contains(result, r => r.Name == "GetAllStatusesTest1");
            Assert.Contains(result, r => r.Name == "GetAllStatusesTest2");
            Assert.Contains(result, r => r.Name == "GetAllStatusesTest3");
            Assert.Contains(result, r => r.Name == "GetAllStatusesTest4");
        }

        [Fact]
        public async Task GetAllAsync_WhenNoTaskItemStatusesExist_ShouldReturnEmptyList()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var repository = CreateTaskItemStatusRepository(dbContext);

            var ct = CancellationToken.None;

            // EnsureCreated() applies entity configurations including seeded data (3 default statuses).
            // To properly test the empty result scenario, we need to remove seeded records first.
            var statuses = await dbContext.TaskItemStatuses.ToListAsync();
            dbContext.RemoveRange(statuses);
            await dbContext.SaveChangesAsync();

            //Act
            var result = await repository.GetAllAsync(ct);

            //Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }


        [Fact]
        public async Task AddAsync_ShouldAddTaskItemStatus()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var repository = CreateTaskItemStatusRepository(dbContext);

            var ct = CancellationToken.None;

            var status = new TaskItemStatus
            {
                Name = "AddStatusTest"
            };

            //Act
            await repository.AddAsync(status, ct);
            await repository.SaveChangesAsync(ct);
            var result = await dbContext.TaskItemStatuses.FindAsync(status.Id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(status.Id, result.Id);
            Assert.Equal("AddStatusTest", result.Name);
        }



        [Fact]
        public async Task Update_WhenTaskItemStatusDoesExists_ShouldUpdateTaskItemStatus()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var repository = CreateTaskItemStatusRepository(dbContext);

            var ct = CancellationToken.None;

            var status = new TaskItemStatus
            {
                Name = "UpdateStatusTest"
            };

            await dbContext.TaskItemStatuses.AddAsync(status);
            await dbContext.SaveChangesAsync();

            //Act
            dbContext.Entry(status).State = EntityState.Detached;

            status.Name = "NewUpdateStatusTest";

            repository.Update(status);
            await repository.SaveChangesAsync(ct);

            using var newDbContext = CreateDbContext(connection);
            var result = await newDbContext.TaskItemStatuses.FindAsync(status.Id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(status.Id, result.Id);
            Assert.Equal("NewUpdateStatusTest", result.Name);
        }




        [Fact]
        public async Task DeleteByIdAsync_WhenTaskItemStatusExists_ShouldDeleteAndReturnTrue()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var repository = CreateTaskItemStatusRepository(dbContext);

            var ct = CancellationToken.None;

            var status = new TaskItemStatus
            {
                Name = "DeleteStatusTest"
            };

            await dbContext.TaskItemStatuses.AddAsync(status);
            await dbContext.SaveChangesAsync();

            //Act
            var resultSucceeded = await repository.DeleteByIdAsync(status.Id, ct);
            await repository.SaveChangesAsync(ct);

            var deleteResult = await dbContext.TaskItemStatuses.FindAsync(status.Id);

            //Assert
            Assert.True(resultSucceeded);
            Assert.Null(deleteResult);
        }

        [Fact]
        public async Task DeleteByIdAsync_WhenTaskItemStatusDoesNotExist_ShouldReturnFalse()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var repository = CreateTaskItemStatusRepository(dbContext);

            var ct = CancellationToken.None;

            //Act
            var resultSucceeded = await repository.DeleteByIdAsync(int.MaxValue, ct);

            //Assert
            Assert.False(resultSucceeded);

        }

        [Fact]
        public async Task ExistsAsync_WhenTaskItemStatusExists_ReturnTrue()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var repository = CreateTaskItemStatusRepository(dbContext);

            var ct = CancellationToken.None;

            var status = new TaskItemStatus
            {
                Name = "ExistsStatusTest"
            };

            await dbContext.TaskItemStatuses.AddAsync(status);
            await dbContext.SaveChangesAsync();

            //Act

            var result = await repository.ExistsAsync(status.Id, ct);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_WhenTaskItemStatusDoesNotExist_ReturnFalse()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var repository = CreateTaskItemStatusRepository(dbContext);

            var ct = CancellationToken.None;

            //Act
            var result = await repository.ExistsAsync(999, ct);

            //Assert
            Assert.False(result);
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

        private TaskItemStatusRepository CreateTaskItemStatusRepository(TaskManagerDbContext dbContext)
        {
            return new TaskItemStatusRepository(dbContext);
        }
    }
}
