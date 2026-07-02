using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagerAPI.AutoMapper;
using TaskManagerAPI.Data;
using TaskManagerAPI.GlobalExceptionHandler.Exceptions.Business;
using TaskManagerAPI.Models;
using TaskManagerAPI.Models.Dtos.TaskItemDtos;
using TaskManagerAPI.Models.Filters;
using TaskManagerAPI.Models.Identity;
using TaskManagerAPI.Repositories;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Tests
{
    public class TaskItemServiceTests
    {
        [Fact]
        public async Task GetAllAsync_NoFilters_UserIsNotAdmin_TaskItemsExist_ShouldReturnUserTaskItemDtos()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var taskItems = CreateTaskItemListForGetAll();

            await dbContext.TaskItems.AddRangeAsync(taskItems);
            await dbContext.SaveChangesAsync(ct);

            var filters = new TaskItemFilters();

            var userInfo = new UserInfoDto()
            {
                UserId = "developerTestId",
                IsAdmin = false
            };

            //Act
            var result = await service.GetAllAsync(filters, userInfo, ct);

            //Assert 
            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.Take);
            Assert.Contains(result.Items, i => i.Title == "GetAllDeveloperTest1");
            Assert.Contains(result.Items, i => i.Title == "GetAllDeveloperTest2");

        }

        [Fact]
        public async Task GetAllAsync_NoFilters_UserIsNotAdmin_TaskItemsDoesNotExist_ShouldReturnEmptyList()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;


            var filters = new TaskItemFilters();

            var userInfo = new UserInfoDto()
            {
                UserId = "developerTestId",
                IsAdmin = false
            };

            //Act
            var result = await service.GetAllAsync(filters, userInfo, ct);

            //Assert 
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.Take);

        }

        [Fact]
        public async Task GetAllAsync_PagingApplied_UserIsNotAdmin_TaskItemsExist_ShouldReturnPagedUserTaskItemDtos()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var taskItems = CreateTaskItemListForGetAll();

            await dbContext.TaskItems.AddRangeAsync(taskItems);
            await dbContext.SaveChangesAsync(ct);

            var filters = new TaskItemFilters()
            {
                Page = 1,
                Take = 1,
            };

            var userInfo = new UserInfoDto()
            {
                UserId = "developerTestId",
                IsAdmin = false
            };

            //Act
            var result = await service.GetAllAsync(filters, userInfo, ct);

            //Assert 
            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Single(result.Items);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(1, result.Page);
            Assert.Equal(1, result.Take);
            Assert.Contains(result.Items, i => i.Title == "GetAllDeveloperTest1");

        }

        [Fact]
        public async Task GetAllAsync_InvalidPaging_UserIsNotAdmin_TaskItemsExist_ShouldNormalizePaging()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var taskItems = CreateTaskItemListForGetAll();

            await dbContext.TaskItems.AddRangeAsync(taskItems);
            await dbContext.SaveChangesAsync(ct);

            var filters = new TaskItemFilters()
            {
                Page = -1,
                Take = 101,
            };

            var userInfo = new UserInfoDto()
            {
                UserId = "developerTestId",
                IsAdmin = false
            };

            //Act
            var result = await service.GetAllAsync(filters, userInfo, ct);

            //Assert 
            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(1, result.Page);
            Assert.Equal(100, result.Take);
            Assert.Contains(result.Items, i => i.Title == "GetAllDeveloperTest1");
            Assert.Contains(result.Items, i => i.Title == "GetAllDeveloperTest2");

        }

        [Fact]
        public async Task GetAllAsync_PagingAndSearch_UserIsNotAdmin_TaskItemsExist_ShouldReturnFilteredUserTaskItemDtos()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var taskItems = CreateTaskItemListForGetAll();

            await dbContext.TaskItems.AddRangeAsync(taskItems);
            await dbContext.SaveChangesAsync(ct);

            var filters = new TaskItemFilters()
            {
                Page = 1,
                Take = 2,
                Title = "1"
            };

            var userInfo = new UserInfoDto()
            {
                UserId = "developerTestId",
                IsAdmin = false
            };

            //Act
            var result = await service.GetAllAsync(filters, userInfo, ct);

            //Assert 
            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);
            Assert.Equal(1, result.Page);
            Assert.Equal(2, result.Take);
            Assert.Contains(result.Items, i => i.Title == "GetAllDeveloperTest1");

        }

        [Fact]
        public async Task GetAllAsync_PageOutOfRange_UserIsNotAdmin_TaskItemsExist_ShouldReturnEmptyList()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var taskItems = CreateTaskItemListForGetAll();

            await dbContext.TaskItems.AddRangeAsync(taskItems);
            await dbContext.SaveChangesAsync(ct);

            var filters = new TaskItemFilters()
            {
                Page = 2,
                Take = 2,
            };

            var userInfo = new UserInfoDto()
            {
                UserId = "developerTestId",
                IsAdmin = false
            };

            //Act
            var result = await service.GetAllAsync(filters, userInfo, ct);

            //Assert 
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Page);
            Assert.Equal(2, result.Take);
          
        }

        [Fact]
        public async Task GetAllAsync_NoFilters_Admin_TaskItemsExist_ShouldReturnAllTaskItemDtos()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var taskItems = CreateTaskItemListForGetAll();

            await dbContext.TaskItems.AddRangeAsync(taskItems);
            await dbContext.SaveChangesAsync(ct);

            var filters = new TaskItemFilters();

            var userInfo = new UserInfoDto()
            {
                UserId = "adminTestId",
                IsAdmin = true,
            };

            //Act
            var result = await service.GetAllAsync(filters, userInfo, ct);

            //Assert 
            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Equal(4, result.Items.Count());
            Assert.Equal(4, result.TotalCount);
            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.Take);
            Assert.Contains(result.Items, i => i.Title == "GetAllDeveloperTest1");
            Assert.Contains(result.Items, i => i.Title == "GetAllDeveloperTest2");
            Assert.Contains(result.Items, i => i.Title == "GetAllTeamLeaderTest1");
            Assert.Contains(result.Items, i => i.Title == "GetAllTeamLeaderTest2");

        }

        [Fact]
        public async Task GetAllAsync_PageOutOfRange_Admin_TaskItemsExist_ShouldReturnEmptyList()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var taskItems = CreateTaskItemListForGetAll();

            await dbContext.TaskItems.AddRangeAsync(taskItems);
            await dbContext.SaveChangesAsync(ct);

            var filters = new TaskItemFilters()
            {
                Page = 2,
                Take = 4,
            };

            var userInfo = new UserInfoDto()
            {
                UserId = "adminTestId",
                IsAdmin = true
            };

            //Act
            var result = await service.GetAllAsync(filters, userInfo, ct);

            //Assert 
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(4, result.TotalCount);
            Assert.Equal(2, result.Page);
            Assert.Equal(4, result.Take);
          
        }

            [Fact]
        public async Task GetAllAsync_PagingAndSearch_Admin_TaskItemsExist_ShouldReturnFilteredTaskItemDtos()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var taskItems = CreateTaskItemListForGetAll();

            await dbContext.TaskItems.AddRangeAsync(taskItems);
            await dbContext.SaveChangesAsync(ct);

            var filters = new TaskItemFilters()
            {
                Page = 1,
                Take = 2,
                Title = "1"
            };

            var userInfo = new UserInfoDto()
            {
                UserId = "adminTestId",
                IsAdmin = true
            };

            //Act
            var result = await service.GetAllAsync(filters, userInfo, ct);

            //Assert 
            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(1, result.Page);
            Assert.Equal(2, result.Take);
            Assert.Contains(result.Items, i => i.Title == "GetAllDeveloperTest1");
            Assert.Contains(result.Items, i => i.Title == "GetAllTeamLeaderTest1");
        }

        [Fact]
        public async Task GetAllAsync_InvalidPaging_Admin_TaskItemsExist_ShouldNormalizePaging()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var taskItems = CreateTaskItemListForGetAll();

            await dbContext.TaskItems.AddRangeAsync(taskItems);
            await dbContext.SaveChangesAsync(ct);

            var filters = new TaskItemFilters()
            {
                Page = -1,
                Take = 101,
            };

            var userInfo = new UserInfoDto()
            {
                UserId = "adminTestId",
                IsAdmin = true
            };

            //Act
            var result = await service.GetAllAsync(filters, userInfo, ct);

            //Assert 
            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Equal(4,result.Items.Count());
            Assert.Equal(4, result.TotalCount);
            Assert.Equal(1, result.Page);
            Assert.Equal(100, result.Take);
            Assert.Contains(result.Items, i => i.Title == "GetAllDeveloperTest1");
            Assert.Contains(result.Items, i => i.Title == "GetAllDeveloperTest2");
            Assert.Contains(result.Items, i => i.Title == "GetAllTeamLeaderTest1");
            Assert.Contains(result.Items, i => i.Title == "GetAllTeamLeaderTest2");

        }

        [Fact]
        public async Task GetByIdAsync_TaskItemExists_ValidUser_ShouldReturnTaskItemDto()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
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
            await dbContext.SaveChangesAsync(ct);

            var userInfo = new UserInfoDto()
            {
                UserId = taskItem.UserId,
                IsAdmin = false
            };

            //Act
            var result = await service.GetByIdAsync(taskItem.Id, userInfo, ct);

            //Assert 
            Assert.NotNull(result);
            Assert.NotNull(result.Status);
            Assert.Null(result.DueDate);
            Assert.Equal(taskItem.Id, result.Id);
            Assert.Equal("GetByIdTest", result.Title);
            Assert.Equal("GetByIdTest", result.Description);
            Assert.Equal(taskItem.CreatedDate, result.CreatedDate);
            Assert.Equal(1, result.Status.Id);

        }

        [Fact]
        public async Task GetByIdAsync_TaskItemExists_Admin_ShouldReturnTaskItemDto()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var taskItem = new TaskItem()
            {
                Title = "GetByIdTest",
                Description = "GetByIdTest",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 1,
                UserId = "teamLeaderTestId"
            };

            await dbContext.TaskItems.AddAsync(taskItem);
            await dbContext.SaveChangesAsync(ct);

            var userInfo = new UserInfoDto()
            {
                UserId = "adminTestId",
                IsAdmin = true
            };

            //Act
            var result = await service.GetByIdAsync(taskItem.Id, userInfo, ct);

            //Assert 
            Assert.NotNull(result);
            Assert.NotNull(result.Status);
            Assert.Null(result.DueDate);
            Assert.Equal(taskItem.Id, result.Id);
            Assert.Equal("GetByIdTest", result.Title);
            Assert.Equal("GetByIdTest", result.Description);
            Assert.Equal(taskItem.CreatedDate, result.CreatedDate);
            Assert.Equal(1, result.Status.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WhenTaskItemDoesNotExists_ShouldThrowNotFoundException()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            UserInfoDto userInfo = new UserInfoDto()
            {
                UserId = "developerTestId",
                IsAdmin = false
            };

            //Act-Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(int.MaxValue, userInfo, ct));

        }

        [Fact]
        public async Task GetByIdAsync_TaskItemExists_InvalidUser_ShouldThrowForbiddenOperationException()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var taskItem = new TaskItem()
            {
                Title = "GetByIdTest",
                Description = "GetByIdTest",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 1,
                UserId = "teamLeaderTestId"
            };

            await dbContext.TaskItems.AddAsync(taskItem);
            await dbContext.SaveChangesAsync(ct);

            var userInfo = new UserInfoDto()
            {
                UserId = "developerTestId",
                IsAdmin = false
            };

            //Act-Assert
            await Assert.ThrowsAsync<ForbiddenOperationException>(() => service.GetByIdAsync(taskItem.Id, userInfo, ct));
        }

        [Fact]
        public async Task AddAsync_WhenDueDateIsInPast_ShouldThrowParameterValidationException()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var taskItem = new CreateTaskItemDto()
            {
                Title = "AddTest",
                Description = "AddTest",
                DueDate = DateTime.UtcNow.AddDays(-1),
                StatusId = 1,
            };

            var userInfo = new UserInfoDto()
            {
                UserId = "teamLeaderTestId",
                IsAdmin = false
            };

            //Act-Assert
            await Assert.ThrowsAsync<ParameterValidationException>(() => service.AddAsync(taskItem, userInfo, ct));
        }


        [Fact]
        public async Task AddAsync_ValidOrNullDueDate_StatusNotExists_ShouldThrowNotFoundException()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var taskItem = new CreateTaskItemDto()
            {
                Title = "AddTest",
                Description = "AddTest",
                DueDate = null,
                StatusId = 4,
            };

            var userInfo = new UserInfoDto()
            {
                UserId = "teamLeaderTestId",
                IsAdmin = false
            };

            //Act-Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.AddAsync(taskItem, userInfo, ct));
        }

        [Fact]
        public async Task AddAsync_ValidOrNullDueDate_StatusExists_ShouldAddTaskItemAndReturnTaskItemDto()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var taskItem = new CreateTaskItemDto()
            {
                Title = "AddTest",
                Description = "AddTest",
                DueDate = null,
                StatusId = 1,
            };

            var userInfo = new UserInfoDto()
            {
                UserId = "teamLeaderTestId",
                IsAdmin = false
            };

            //Act
            var result = await service.AddAsync(taskItem, userInfo, ct);

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Status);
            Assert.Null(result.DueDate);
            Assert.Equal("AddTest", result.Title);
            Assert.Equal("AddTest", result.Description);
            Assert.Equal(taskItem.DueDate, result.DueDate);
            Assert.Equal(1, result.Status.Id);
        }

        [Fact]
        public async Task UpdateAsync_WhenIdsDoNotMatch_ShouldThrowArgumentMismatchException()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var updateDto = new UpdateTaskItemDto
            {
                Id = 1,
                Title = "UpdateTest",
                Description = "UpdateTest",
                DueDate = null,
                StatusId = 1,
            };

            var userInfo = new UserInfoDto
            {
                UserId = "developerTestId",
                IsAdmin = false,
            };

            //Act-Assert
            await Assert.ThrowsAsync<ArgumentMismatchException>(() => service.UpdateAsync(int.MaxValue, updateDto, userInfo, ct));
        }

        [Fact]
        public async Task UpdateAsync_IdsMatch_DueDateIsInPast_ShouldThrowParameterValidationException()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var updateDto = new UpdateTaskItemDto
            {
                Id = 1,
                Title = "UpdateTest",
                Description = "UpdateTest",
                DueDate = DateTime.UtcNow.AddDays(-1),
                StatusId = 1,
            };

            var userInfo = new UserInfoDto
            {
                UserId = "developerTestId",
                IsAdmin = false,
            };

            //Act-Assert
            await Assert.ThrowsAsync<ParameterValidationException>(() => service.UpdateAsync(1, updateDto, userInfo, ct));
        }

        [Fact]
        public async Task UpdateAsync_IdsMatch_ValidOrNullDueDate_InvalidStatusId_ShouldThrowNotFoundException()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var updateDto = new UpdateTaskItemDto
            {
                Id = 1,
                Title = "UpdateTest",
                Description = "UpdateTest",
                DueDate = null,
                StatusId = 4,
            };

            var userInfo = new UserInfoDto
            {
                UserId = "developerTestId",
                IsAdmin = false,
            };

            //Act-Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateAsync(1, updateDto, userInfo, ct));
        }

        [Fact]
        public async Task UpdateAsync_IdsMatch_ValidOrNullDueDate_ValidStatusId_TaskDoesNotExists_ShouldThrowNotFoundException()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var updateDto = new UpdateTaskItemDto
            {
                Id = 1,
                Title = "UpdateTest",
                Description = "UpdateTest",
                DueDate = null,
                StatusId = 3,
            };

            var userInfo = new UserInfoDto
            {
                UserId = "developerTestId",
                IsAdmin = false,
            };

            //Act-Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateAsync(1, updateDto, userInfo, ct));
        }

        [Fact]
        public async Task UpdateAsync_IdsMatch_ValidOrNullDueDate_ValidStatusId_TaskExists_InvalidUser_ShouldThrowForbiddenOperationException()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var taskItem = new TaskItem
            {
                Title = "UpdateTest",
                Description = "UpdateTest",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 1,
                UserId = "teamLeaderTestId"
            };

            await dbContext.TaskItems.AddAsync(taskItem);
            await dbContext.SaveChangesAsync();

            var updateDto = new UpdateTaskItemDto
            {
                Id = taskItem.Id,
                Title = "UpdateTestDeveloper",
                Description = "UpdateTestDeveloper",
                DueDate = null,
                StatusId = 3,
            };

            var userInfo = new UserInfoDto
            {
                UserId = "developerTestId",
                IsAdmin = false,
            };

            //Act-Assert
            await Assert.ThrowsAsync<ForbiddenOperationException>(() => service.UpdateAsync(updateDto.Id, updateDto, userInfo, ct));
        }

        [Fact]
        public async Task UpdateAsync_IdsMatch_ValidOrNullDueDate_ValidStatusId_TaskItemExists_ValidUser_ShouldUpdateTaskItem()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var taskItem = new TaskItem
            {
                Title = "UpdateTest",
                Description = "UpdateTest",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 1,
                UserId = "teamLeaderTestId"
            };

            await dbContext.TaskItems.AddAsync(taskItem);
            await dbContext.SaveChangesAsync();

            var updateDto = new UpdateTaskItemDto
            {
                Id = taskItem.Id,
                Title = "UpdateTestTeamLeader",
                Description = "UpdateTestTeamLeader",
                DueDate = DateTime.UtcNow.AddDays(1),
                StatusId = 3,
            };

            var userInfo = new UserInfoDto
            {
                UserId = "teamLeaderTestId",
                IsAdmin = false,
            };

            //Act
            await service.UpdateAsync(updateDto.Id, updateDto, userInfo, ct);

            var result = await dbContext.TaskItems.Include(ti => ti.Status).Include(ti => ti.User).FirstOrDefaultAsync(ti => ti.Id == updateDto.Id);

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Status);
            Assert.NotNull(result.User);
            Assert.Equal(taskItem.Id, result.Id);
            Assert.Equal("UpdateTestTeamLeader", result.Title);
            Assert.Equal("UpdateTestTeamLeader", result.Description);
            Assert.Equal(updateDto.DueDate, result.DueDate);
            Assert.Equal(3, result.StatusId);
            Assert.Equal("teamLeaderTestId", result.UserId);


        }

        [Fact]
        public async Task UpdateAsync_IdsMatch_ValidOrNullDueDate_ValidStatusId_TaskExists_Admin_ShouldUpdateTaskItem()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var taskItem = new TaskItem
            {
                Title = "UpdateTest",
                Description = "UpdateTest",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 1,
                UserId = "teamLeaderTestId"
            };

            await dbContext.TaskItems.AddAsync(taskItem);
            await dbContext.SaveChangesAsync();

            var updateDto = new UpdateTaskItemDto
            {
                Id = taskItem.Id,
                Title = "UpdateTestAdmin",
                Description = "UpdateTestAdmin",
                DueDate = DateTime.UtcNow.AddDays(1),
                StatusId = 3,
            };

            var userInfo = new UserInfoDto
            {
                UserId = "adminTestId",
                IsAdmin = true,
            };

            //Act
            await service.UpdateAsync(updateDto.Id, updateDto, userInfo, ct);

            var result = await dbContext.TaskItems.Include(ti => ti.Status).Include(ti => ti.User).FirstOrDefaultAsync(ti => ti.Id == updateDto.Id);

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Status);
            Assert.NotNull(result.User);
            Assert.Equal(taskItem.Id, result.Id);
            Assert.Equal("UpdateTestAdmin", result.Title);
            Assert.Equal("UpdateTestAdmin", result.Description);
            Assert.Equal(updateDto.DueDate, result.DueDate);
            Assert.Equal(3, result.StatusId);
            Assert.Equal("teamLeaderTestId", result.UserId);

        }

        [Fact]
        public async Task DeleteAsync_WhenTaskItemDoesNotExists_ShouldThrowNotFoundException()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var userInfo = new UserInfoDto
            {
                UserId = "developerTestId",
                IsAdmin = true,
            };

            //Act-Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync(int.MaxValue, userInfo, ct));

        }

        [Fact]
        public async Task DeleteAsync_TaskItemExists_InvalidUser_ShouldThrowForbiddenOperationException()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var taskItem = new TaskItem
            {
                Title = "DeleteTest",
                Description = "DeleteTest",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 1,
                UserId = "teamLeaderTestId"
            };

            await dbContext.TaskItems.AddAsync(taskItem);
            await dbContext.SaveChangesAsync();

            var userInfo = new UserInfoDto
            {
                UserId = "developerTestId",
                IsAdmin = false,
            };

            //Act-Assert
            await Assert.ThrowsAsync<ForbiddenOperationException>(() => service.DeleteAsync(taskItem.Id, userInfo, ct));

        }


        [Fact]
        public async Task DeleteAsync_TaskItemExists_ValidUser_ShouldDeleteTaskItem()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var taskItem = new TaskItem
            {
                Title = "DeleteTest",
                Description = "DeleteTest",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 1,
                UserId = "teamLeaderTestId"
            };

            await dbContext.TaskItems.AddAsync(taskItem);
            await dbContext.SaveChangesAsync();

            var userInfo = new UserInfoDto
            {
                UserId = "teamLeaderTestId",
                IsAdmin = false,
            };

            //Act
            await service.DeleteAsync(taskItem.Id, userInfo, ct);

            var result = await dbContext.TaskItems.FirstOrDefaultAsync(ti => ti.Id == taskItem.Id);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_TaskItemExists_Admin_ShouldDeleteTaskItem()
        {
            //Arrange
            using var connection = CreateConnection();
            using var dbContext = CreateDbContent(connection);
            var service = await CreateSeededservice(dbContext);
            var ct = CancellationToken.None;

            var taskItem = new TaskItem
            {
                Title = "DeleteTest",
                Description = "DeleteTest",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 1,
                UserId = "teamLeaderTestId"
            };

            await dbContext.TaskItems.AddAsync(taskItem);
            await dbContext.SaveChangesAsync();

            var userInfo = new UserInfoDto
            {
                UserId = "adminTestId",
                IsAdmin = true,
            };

            //Act
            await service.DeleteAsync(taskItem.Id, userInfo, ct);

            var result = await dbContext.TaskItems.FirstOrDefaultAsync(ti => ti.Id == taskItem.Id);

            //Assert
            Assert.Null(result);
        }


        private SqliteConnection CreateConnection()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            return connection;
        }

        private TaskManagerDbContext CreateDbContent(SqliteConnection connection)
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

        private TaskItemStatusRepository CreateTaskItemStatusRepository(TaskManagerDbContext dbContext)
        {
            return new TaskItemStatusRepository(dbContext);
        }

        private IMapper CreateAutoMapper()
        {
            var loggerFactory = LoggerFactory.Create(builder => { });
            var config = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); }, loggerFactory);


            return config.CreateMapper();
        }

        private TaskItemService CreateService(TaskItemRepository taskItemRepository, TaskItemStatusRepository taskItemStatusRepository, IMapper autoMapper)
        {
            return new TaskItemService(taskItemRepository, taskItemStatusRepository, autoMapper);
        }

        private async Task<TaskItemService> CreateSeededservice(TaskManagerDbContext dbContext)
        {
            await SeedData(dbContext);
            var taskItemRepository = CreateTaskItemRepository(dbContext);
            var taskItemStatusRepository = CreateTaskItemStatusRepository(dbContext);
            var autoMapper = CreateAutoMapper();
            return CreateService(taskItemRepository, taskItemStatusRepository, autoMapper);
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

        private List<TaskItem> CreateTaskItemListForGetAll()
        {
            return new List<TaskItem>
            {
                new TaskItem()
            {
                Title = "GetAllDeveloperTest1",
                Description = "GetAllDeveloperTest1",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 1,
                UserId = "developerTestId"
            },
                new TaskItem()
            {
                Title = "GetAllDeveloperTest2",
                Description = "GetAllDeveloperTest2",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 1,
                UserId = "developerTestId"
            },
                new TaskItem()
            {
                Title = "GetAllTeamLeaderTest1",
                Description = "GetAllDeveloperTest1",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 1,
                UserId = "teamLeaderTestId"
            },
                new TaskItem()
            {
                Title = "GetAllTeamLeaderTest2",
                Description = "GetAllDeveloperTest2",
                CreatedDate = DateTime.UtcNow,
                DueDate = null,
                StatusId = 1,
                UserId = "teamLeaderTestId"
            }
            };
        }



    }
}
