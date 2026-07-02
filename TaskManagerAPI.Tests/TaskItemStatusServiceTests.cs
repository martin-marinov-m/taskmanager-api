using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagerAPI.AutoMapper;
using TaskManagerAPI.Data;
using TaskManagerAPI.GlobalExceptionHandler.Exceptions.Business;
using TaskManagerAPI.Models;
using TaskManagerAPI.Models.Dtos;
using TaskManagerAPI.Models.Dtos.TaskItemStatusDtos;
using TaskManagerAPI.Repositories;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Tests
{
    public class TaskItemStatusServiceTests
    {

        [Fact]
        public async Task GetAllAsync_WhenTaskItemStatusesExist_ShouldReturnTaskItemStatusDtos()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var service = CreateTaskItemStatusService(dbContext);

            var ct = CancellationToken.None;

            var statuses = new List<TaskItemStatus>
            {
                new TaskItemStatus
            {
                Name = "GetAllTest1"
            },
                new TaskItemStatus
            {
                Name = "GetAllTest2"
            },
                new TaskItemStatus
            {
                Name = "GetAllTest3"
            },
                new TaskItemStatus
            {
                Name = "GetAllTest4"
            }
            };

            await dbContext.AddRangeAsync(statuses);
            await dbContext.SaveChangesAsync();

            //Act
            var result = await service.GetAllAsync(ct);

            //Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains(result, r => r.Name == "GetAllTest1");
            Assert.Contains(result, r => r.Name == "GetAllTest2");
            Assert.Contains(result, r => r.Name == "GetAllTest3");
            Assert.Contains(result, r => r.Name == "GetAllTest4");
        }

        [Fact]
        public async Task GetAllAsync_WhenTaskItemStatusesDoNotExist_ShouldReturnEmptyList()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var service = CreateTaskItemStatusService(dbContext);

            var ct = CancellationToken.None;

            var statuses = await dbContext.TaskItemStatuses.ToListAsync();
            dbContext.RemoveRange(statuses);
            await dbContext.SaveChangesAsync();

            //Act
            var result = await service.GetAllAsync(ct);

            //Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }




        [Fact]
        public async Task GetByIdAsync_WhenTaskItemStatusIsFound_ShouldReturnTaskItemStatusDto()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var service = CreateTaskItemStatusService(dbContext);

            var ct = CancellationToken.None;

            var status = new TaskItemStatus
            {
                Name = "GetByIdTest"
            };

            await dbContext.TaskItemStatuses.AddAsync(status);
            await dbContext.SaveChangesAsync();

            //Act
            var result = await service.GetByIdAsync(status.Id, ct);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(status.Id, result.Id);
            Assert.Equal("GetByIdTest", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WhenTaskItemStatusIsNotFound_ShouldThrowNotFoundException()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var service = CreateTaskItemStatusService(dbContext);

            var ct = CancellationToken.None;

            //Act-Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(int.MaxValue, ct));

        }

        [Fact]
        public async Task AddAsync_WhenTaskItemStatusIsCreatedSuccessfully_ShouldReturnTaskItemStatusDto()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var service = CreateTaskItemStatusService(dbContext);

            var ct = CancellationToken.None;

            var status = new CreateTaskItemStatusDto
            {
                Name = "CreateTest"
            };

            //Act
            var resultCreate = await service.AddAsync(status, ct);

            var findResult = await dbContext.TaskItemStatuses.FindAsync(resultCreate.Id);

            //Assert
            Assert.NotNull(resultCreate);
            Assert.Equal("CreateTest", resultCreate.Name);
            Assert.NotNull(findResult);
            Assert.Equal("CreateTest", findResult.Name);
        }

        [Fact]
        public async Task UpdateAsync_WhenIdsDoNotMatch_ShouldThrowArgumentMismatchException()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var service = CreateTaskItemStatusService(dbContext);

            var ct = CancellationToken.None;

            var status = new TaskItemStatus
            {
                Name = "PreUpdateTest"
            };

            await dbContext.TaskItemStatuses.AddAsync(status);
            await dbContext.SaveChangesAsync();

            var updateDto = new TaskItemStatusDto
            {
                Id = 4,
                Name = "PostUpdateTest"
            };

            //Act-Assert

            await Assert.ThrowsAsync<ArgumentMismatchException>(() => service.UpdateAsync(int.MaxValue, updateDto, ct));

        }

        [Fact]
        public async Task UpdateAsync_WhenIdsMatchAndTaskItemStatusDoesNotExist_ShouldThrowNotFoundException()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var service = CreateTaskItemStatusService(dbContext);

            var ct = CancellationToken.None;

            var updateDto = new TaskItemStatusDto
            {
                Id = 4,
                Name = "PostUpdateTest"
            };

            //Act-Assert

            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateAsync(updateDto.Id, updateDto, ct));

        }

        [Fact]
        public async Task UpdateAsync_WhenIdsMatchAndTaskItemStatusExists_ShouldUpdateTaskItemStatus()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var service = CreateTaskItemStatusService(dbContext);

            var ct = CancellationToken.None;


            var status = new TaskItemStatus
            {
                Name = "PreUpdateTest"
            };

            await dbContext.TaskItemStatuses.AddAsync(status);
            await dbContext.SaveChangesAsync();

            //Act

            var updateDto = new TaskItemStatusDto
            {
                Id = status.Id,
                Name = "PostUpdateTest"
            };

            await service.UpdateAsync(updateDto.Id, updateDto, ct);

            var result = await dbContext.TaskItemStatuses.FindAsync(status.Id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(status.Id, result.Id);
            Assert.Equal("PostUpdateTest", result.Name);
        }

        [Fact]
        public async Task DeleteAsync_WhenTaskItemStatusDoesNotExist_ShouldThrowNotFoundException()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var service = CreateTaskItemStatusService(dbContext);

            var ct = CancellationToken.None;

            //Act-Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync(int.MaxValue, ct));
        }



        [Fact]
        public async Task DeleteAsync_WhenTaskItemStatusExists_ShouldDeleteTaskItemStatus()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContext(connection);
            var service = CreateTaskItemStatusService(dbContext);

            var ct = CancellationToken.None;

            var status = new TaskItemStatus
            {
                Name = "DeleteTest"
            };

            await dbContext.TaskItemStatuses.AddAsync(status);
            await dbContext.SaveChangesAsync();

            //Act
            await service.DeleteAsync(status.Id, ct);

            var result = await dbContext.TaskItemStatuses.FindAsync(status.Id);

            //Assert
            Assert.Null(result);

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

        private TaskItemStatusService CreateTaskItemStatusService(TaskManagerDbContext dbContext)
        {
            var taskItemStatusRepository = CreateTaskItemStatusRepository(dbContext);
            var mapper = CreateAutoMapper();
            return new TaskItemStatusService(taskItemStatusRepository, mapper);
        }

        private IMapper CreateAutoMapper()
        {
            var loggerFactory = LoggerFactory.Create(cfg => { });
            var mapper = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); }, loggerFactory);
            var autoMapper = mapper.CreateMapper();
            return autoMapper;
        }

    }
}
